using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
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
        public Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Supplier>> AllSuppliersAsync()
        {

            List<Supplier> suppliers = new List<Supplier>();

            List<SupplierProductRow> rows = new List<SupplierProductRow>();
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("select s.id, s.name, s.legal_entity, s.address, sp.price, sp.product_id, p.name from suppliers s " +
                    "left join supplier_product sp on s.id = sp.supplier_id left join products p on sp.product_id = p.id", conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        rows.Add(new SupplierProductRow() {
                            SupplierId = reader.GetInt32(0), 
                            SupplierName = reader.GetString(1),
                            LegalName = reader.GetString(2),
                            Address = reader.GetString(3),
                            Price = reader.SafeGetString(4),
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
                    LegalName = group.First().LegalName,
                    Address = group.First().Address
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

        public Task DeleteSupplierAsync(int carCategoryId)
        {
            throw new NotImplementedException();
        }

        public Task<Supplier> EditSupplierAsync(Supplier supplier)
        {
            throw new NotImplementedException();
        }

        private class SupplierProductRow {
            public int SupplierId { get; set; }
            public string SupplierName { get; set; }
            public string LegalName { get; set; }
            public string Address { get; set; }
            public int? ProductId { get; set; }
            public string ProductName { get; set; }
            public string Price { get; set; }
        }
    }
}
