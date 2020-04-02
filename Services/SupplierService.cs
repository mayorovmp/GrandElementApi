using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

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
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("insert into suppliers(legal_entity, address, name, vat) values (@LegalEntity, @Address, @Name, @vat) RETURNING id", conn))
                {
                    cmd.Parameters.AddRange(new[] { 
                        new NpgsqlParameter<string>("LegalEntity", supplier.LegalEntity),
                        new NpgsqlParameter<string>("Address", supplier.Address),
                        new NpgsqlParameter<string>("Name", supplier.Name),
                        new NpgsqlParameter("vat", supplier.VAT ? 1 : 0),
                    });
                    

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            supplier.Id = reader.GetInt32(0);
                        }
                    }
                }
                
                foreach (var prod in supplier.Products)
                {
                    AddSupplierProduct(conn, prod, supplier.Id.Value);
                }
                tran.Complete();
            }
            return supplier;
        }

        private void AddSupplierProduct(NpgsqlConnection conn, Product prod, int supplierId)
        {
            if (!prod.Id.HasValue)
            {
                throw new Exception("Идентификатор товара не может быть пустым");
            }
            var cmd = new NpgsqlCommand(@"
insert into supplier_product(supplier_id, product_id, price) values(@supId, @prodId, @price) 
RETURNING supplier_id, product_id, price", conn);
            {
                cmd.Parameters.AddRange(new[] {
                        new NpgsqlParameter<int>("prodId", prod.Id.Value),
                        new NpgsqlParameter<int>("supId", supplierId),
                        prod.Price == null ? new NpgsqlParameter("price", DBNull.Value) : new NpgsqlParameter("price", prod.Price)
                    });
                cmd.ExecuteNonQuery();
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
select s.id, s.name, s.legal_entity, s.address, sp.price, sp.product_id, p.name, s.vat
from suppliers s
    left join supplier_product sp on s.id = sp.supplier_id and sp.row_status = 0
    left join products p on p.id = sp.product_id
where s.row_status=0
order by s.created desc
", conn))
                {
                    using var reader = await cmd.ExecuteReaderAsync();
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
                            ProductName = reader.SafeGetString(6),
                            VAT = reader.SafeGetDecimal(7).GetValueOrDefault(1) == 1 ? true : false,
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
                    Products = new List<Product>(),
                    VAT = group.First().VAT.Value
                    
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
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand(@"
select s.id, s.name, s.legal_entity, s.address, sp.price, sp.product_id, p.name, s.vat
from suppliers s
    left join supplier_product sp on s.id = sp.supplier_id and sp.row_status=0
    left join products p on p.id = sp.product_id
where s.id = @id
and s.row_status=0
order by s.id desc
", conn))
                {
                    cmd.Parameters.AddWithValue("id", NpgsqlDbType.Integer, id);
                    using (var reader = await cmd.ExecuteReaderAsync())
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
                                ProductName = reader.SafeGetString(6),
                                VAT = reader.SafeGetDecimal(7).GetValueOrDefault(1) == 1 ? true : false,
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
                Products = new List<Product>(),
                VAT = rows.First().VAT.GetValueOrDefault(true)
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
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("update suppliers set row_status = 1 where id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    var affected = await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private void DeleteSupplierProduct(NpgsqlConnection conn, int supplierId)
        {
            using (var cmd = new NpgsqlCommand(@"
update supplier_product set 
row_status=1
where supplier_id = @id", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<int>("id", supplierId));
                var updatedRowsCnt = cmd.ExecuteNonQuery();
            }
        }

        private void UpdateSupplier(NpgsqlConnection conn, Supplier supplier) {
            using (var cmd = new NpgsqlCommand(@"
update suppliers set 
legal_entity=@legal_entity, 
address=@address,
name=@name,
vat=@vat
where id = @id returning id, name, legal_entity, address", conn))
            {
                cmd.Parameters.AddRange(new[] {
                    new NpgsqlParameter<int>("id", supplier.Id.Value),
                    new NpgsqlParameter<string>("legal_entity", supplier.LegalEntity),
                    new NpgsqlParameter<string>("name", supplier.Name),
                    new NpgsqlParameter<string>("address", supplier.Address),
                    new NpgsqlParameter("vat", supplier.VAT ? 1 : 0),
                });
                cmd.ExecuteNonQuery();
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

            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = _connectionService.GetOpenedConnection())
            {
                DeleteSupplierProduct(conn, id);
                UpdateSupplier(conn, supplier);

                foreach (var prod in supplier.Products)
                {
                    AddSupplierProduct(conn, prod, supplier.Id.Value);
                }
                tran.Complete();
            }

            return await GetSupplier(id);
        }

        public async Task<List<Supplier>> SuppliersByProductIdAsync(int productId)
        {

            List<Supplier> suppliers = new List<Supplier>();

            List<SupplierProductRow> rows = new List<SupplierProductRow>();
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select s.id, s.name, s.legal_entity, s.address, sp.price, sp.product_id, p.name, s.vat
from suppliers s
    left join supplier_product sp on s.id = sp.supplier_id and sp.row_status = 0
    left join products p on p.id = sp.product_id
where s.row_status=0 and p.id = @productId
", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<Int32>("productId", productId));
                    using var reader = await cmd.ExecuteReaderAsync();
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
                            ProductName = reader.SafeGetString(6),
                            VAT = reader.SafeGetDecimal(7).GetValueOrDefault(1) == 1 ? true : false,
                        });
                    }
                }
            }
            var groups = rows.GroupBy(x => x.SupplierId);
            foreach (var group in groups)
            {
                var supplier = new Supplier()
                {
                    Id = group.Key,
                    Name = group.First().SupplierName,
                    LegalEntity = group.First().LegalName,
                    Address = group.First().Address,
                    Products = new List<Product>(),
                    VAT = group.First().VAT.GetValueOrDefault(true)

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

        private class SupplierProductRow {
            public int SupplierId { get; set; }
            public string SupplierName { get; set; }
            public string LegalName { get; set; }
            public string Address { get; set; }
            public int? ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal? Price { get; set; }
            public bool? VAT { get; set; }
        }
    }
}
