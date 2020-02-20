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
                using (var cmd = new NpgsqlCommand("select id, name from products where row_status=0 order by id", conn))
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

        public async Task<ProductShort> AddProductAsync(ProductShort product)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("insert into products(name) values(@name) RETURNING id, name", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", product.Name));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new ProductShort() { Id = reader.GetInt32(0), Name = reader.SafeGetString(1) };
                    }
                    else
                        throw new Exception("Товар не создан");
                }
            }
        }
        public async Task DeleteProductAsync(int productId)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("update products set row_status=1 where id = @id returning id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", productId));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (!reader.HasRows)
                    {
                        throw new Exception("Товар не удален");
                    }
                }
            }

        }
        public async Task<ProductShort> EditProductAsync(ProductShort product)
        {
            int id;
            if (product.Id.HasValue)
                id = product.Id.Value;
            else
                throw new ArgumentException("Идентификатор записи пустой");
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("update products set name=@name where id = @id returning id, name", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", product.Name));
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", id));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new ProductShort() { Id = reader.SafeGetInt32(0), Name = reader.SafeGetString(1) };
                    }
                    else
                    {
                        throw new Exception("Товар не изменен");
                    }
                }
            }

        }
    }
}
