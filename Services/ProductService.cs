using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class ProductService
    {
        private readonly IConnectionService _connectionService;
        public ProductService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }
        public async Task<List<ProductShort>> AllProductsAsync()
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("select id, name from products", conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    var res = new List<ProductShort>();
                    while (reader.Read())
                    {
                        res.Add(new ProductShort() { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                    return res;
                }
            }

        }
    }
}
