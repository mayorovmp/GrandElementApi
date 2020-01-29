using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IConnectionService _connectionService;
        public SupplierService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("insert into suppliers(legal_entity, address, name) values (@LegalEntity, @Address, @Name) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("LegalEntity", supplier.LegalEntity));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("Address", supplier.Address));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("Name", supplier.Name));
                    
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        supplier.Id = reader.GetInt32(0);
                    }
                    else {
                        throw new Exception("Запись не создана");
                    }
                }
            }
            var supProdTasks = new List<Task>();
            foreach (var prod in supplier.Products)
            {
                supProdTasks.Add(AddSupplierProductAsync(prod, supplier.Id.Value));
            }
            await Task.WhenAll(supProdTasks);
            return supplier;
        }

        private async Task AddSupplierProductAsync(Product prod, int supplierId)
        {
            if (!prod.Id.HasValue)
            {
                throw new Exception("Идентификатор товара не может быть пустым");
            }
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
insert into supplier_product(supplier_id, product_id, price) values(@supId, @prodId, @price) 
RETURNING supplier_id, product_id, price", conn))
                {
                    cmd.Parameters.AddRange(new [] { 
                        new NpgsqlParameter<int>("prodId", prod.Id.Value),
                        new NpgsqlParameter<int>("supId", supplierId),
                        prod.Price == null ? new NpgsqlParameter("price", DBNull.Value) : new NpgsqlParameter("price", prod.Price)
                    });
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Supplier>> AllSuppliersAsync()
        {

            List<Supplier> suppliers = new List<Supplier>();

            List<SupplierProductRow> rows = new List<SupplierProductRow>();
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select s.id, s.name, s.legal_entity, s.address, sp.price, sp.product_id, p.name
from suppliers s
    left join supplier_product sp on s.id = sp.supplier_id and sp.row_status = 0
    left join products p on p.id = sp.product_id
where s.row_status=0
", conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        rows.Add(new SupplierProductRow() {
                            SupplierId = reader.GetInt32(0), 
                            SupplierName = reader.SafeGetString(1),
                            LegalName = reader.SafeGetString(2),
                            Address = reader.SafeGetString(3),
                            Price = reader.SafeGetDecimal(4),
                            ProductId = reader.SafeGetInt32(5),
                            ProductName = reader.SafeGetString(6)
                        });
                    }
                }
            }
            var groups = rows.GroupBy(x=>x.SupplierId);
            foreach(var group in groups)
            {
                var supplier = new Supplier()
                {
                    Id = group.Key,
                    Name = group.First().SupplierName,
                    LegalEntity = group.First().LegalName,
                    Address = group.First().Address,
                    Products = new List<Product>()
                    
                };
                foreach (var row in group)
                {
                    if (row.ProductId.HasValue)
                    {
                        supplier.Products.Add(new Product() { Id = row.ProductId, Name = row.ProductName, Price = row.Price });
                    }
                }
                suppliers.Add(supplier);
            }
            return suppliers;
        }

        public async Task<Supplier> GetSupplier(int id)
        {
            List<SupplierProductRow> rows = new List<SupplierProductRow>();
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select s.id, s.name, s.legal_entity, s.address, sp.price, sp.product_id, p.name
from suppliers s
    left join supplier_product sp on s.id = sp.supplier_id and sp.row_status=0
    left join products p on p.id = sp.product_id
where s.id = @id
and s.row_status=0
", conn))
                {
                    cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        rows.Add(new SupplierProductRow()
                        {
                            SupplierId = reader.GetInt32(0),
                            SupplierName = reader.SafeGetString(1),
                            LegalName = reader.SafeGetString(2),
                            Address = reader.SafeGetString(3),
                            Price = reader.SafeGetDecimal(4),
                            ProductId = reader.SafeGetInt32(5),
                            ProductName = reader.SafeGetString(6)
                        });
                    }
                }
            }
            if (rows.Count == 0)
                throw new ArgumentException("Идентификатор не найден");
            var groups = rows.GroupBy(x => x.SupplierId);
            var supplier = new Supplier()
            {
                Id = rows.First().SupplierId,
                Name = rows.First().SupplierName,
                LegalEntity = rows.First().LegalName,
                Address = rows.First().Address,
                Products = new List<Product>()
            };
            foreach (var row in rows)
            {
                if (row.ProductId.HasValue)
                {
                    supplier.Products.Add(new Product() { Id = row.ProductId, Name = row.ProductName, Price = row.Price });
                }
            }
            return supplier;
        }
        public async Task DeleteSupplierAsync(int id)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("update suppliers set row_status = 1 where id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    var reader = await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task DeleteSupplierProductAsync(int supplierId)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();//legal_entity, address, name
                using (var cmd = new NpgsqlCommand(@"
update supplier_product set 
row_status=1
where supplier_id = @id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", supplierId));
                    var updatedRowsCnt = await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UpdateSupplierAsync(Supplier supplier) {

            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();//legal_entity, address, name
                using (var cmd = new NpgsqlCommand(@"
update suppliers set 
legal_entity=@legal_entity, 
address=@address,
name=@name
where id = @id returning id, name, legal_entity, address", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", supplier.Id.Value));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("legal_entity", supplier.LegalEntity));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", supplier.Name));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("address", supplier.Address));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (!reader.HasRows)
                    {
                        throw new Exception("Запись не изменена");
                    }
                }
            }
        }

        public async Task<Supplier> EditSupplierAsync(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException("Поставщик не может быть пустым");

            int id;
            if (supplier.Id.HasValue)
                id = supplier.Id.Value;
            else
                throw new ArgumentException("Идентификатор записи пустой");

            var deleteSupProdTask = DeleteSupplierProductAsync(id);
            var updateSupTask = UpdateSupplierAsync(supplier);

            await Task.WhenAll(new []{ deleteSupProdTask, updateSupTask });

            var supProdTasks = new List<Task>();
            foreach (var prod in supplier.Products)
            {
                supProdTasks.Add(AddSupplierProductAsync(prod, supplier.Id.Value));
            }
            await Task.WhenAll(supProdTasks);
            return await GetSupplier(id);
        }

        private class SupplierProductRow {
            public int SupplierId { get; set; }
            public string SupplierName { get; set; }
            public string LegalName { get; set; }
            public string Address { get; set; }
            public int? ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal? Price { get; set; }
        }
    }
}
