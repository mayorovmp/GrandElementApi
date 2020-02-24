﻿using GrandElementApi.Extensions;
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
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", name));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        CarCategory category = new CarCategory() { Id = reader.GetInt32(0), Name = reader.SafeGetString(1) };
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
                        categories.Add(new CarCategory() { Id = reader.GetInt32(0), Name = reader.SafeGetString(1) });
                    }
                    return categories;
                }
            }
        }

        public async Task<CarCategory> EditCategoryAsync(CarCategory category)
        {

            int id;
            if (category.Id.HasValue)
                id = category.Id.Value;
            else
                throw new ArgumentException("Идентификатор записи пустой");

            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("update car_categories set name = @name where id=@id returning id, name", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("name", category.Name));
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", id));
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new CarCategory() { Id = reader.SafeGetInt32(0), Name = reader.SafeGetString(1) };
                    }
                    else
                    {
                        throw new Exception("Запись не изменена");
                    }
                }
            }
        }

        public async Task DeleteCategoryAsync(int carCategoryId)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("update car_categories set row_status = 1 where id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", carCategoryId);
                    var reader = await cmd.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
