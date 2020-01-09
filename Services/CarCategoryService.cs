using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class CarCategoryService : ICarCategoryService
    {
        private readonly IConnectionService _connectionService;
        public CarCategoryService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<CarCategory> AddCategoryAsync(string name)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("insert into car_categories(name, row_status) values(@name, 0) RETURNING id, name", conn))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        CarCategory category = new CarCategory() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                        return category;
                    }
                    else {
                        throw new Exception("Категория не создана.");
                    }
                }
            }
        }

        public async Task<List<CarCategory>> AllCategoriesAsync()
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("select id, name from car_categories where row_status = 0", conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    var categories = new List<CarCategory>();
                    while (reader.Read())
                    {
                        categories.Add(new CarCategory() { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                    return categories;
                }
            }
        }

        public Task<CarCategory> EditCategoryAsync(int carCategoryId, string newName)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteCategoryAsync(int carCategoryId)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                //using (var cmd = new NpgsqlCommand("delete from car_categories cc where cc.id = @id", conn))
                using (var cmd = new NpgsqlCommand("update car_categories set row_status = 1 where id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", carCategoryId);
                    var reader = await cmd.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
