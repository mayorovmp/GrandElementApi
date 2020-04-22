using GrandElementApi.Data;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Microsoft.EntityFrameworkCore;
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
        //private readonly ApplicationContext _db;
        public CarCategoryService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<CarCategory> AddCategoryAsync(CarCategory category)
        {
            using (var db = new ApplicationContext()) {
                if (db.CarCategories.Contains(category))
                    throw new Exception("Запись уже существует");
                db.CarCategories.Add(category);
                await db.SaveChangesAsync();
                return category;
            }
        }

        public async Task<List<CarCategory>> AllCategoriesAsync2()
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

        public async Task<List<CarCategory>> AllCategoriesAsync()
        {
            using var db = new ApplicationContext();
            var res = await db.CarCategories.Where(x=> x.RowStatus == RowStatus.Active).Include(x=>x.Cars).ToListAsync();
            return res;
        }

        public async Task<CarCategory> EditCategoryAsync(CarCategory category) {
            using var db = new ApplicationContext();
            var prevCat = db.CarCategories.FirstOrDefault(x => x.Id == category.Id);
            if (prevCat == null)
                throw new Exception("Категория не найдена");
            prevCat.Name = category.Name;
            await db.SaveChangesAsync();
            return prevCat;
        }

        public async Task DeleteCategoryAsync(int carCategoryId)
        {
            using var db = new ApplicationContext();
            var category = db.CarCategories.FirstOrDefault(x => x.Id == carCategoryId);
            if (category != null)
                category.RowStatus = RowStatus.Removed;
            await db.SaveChangesAsync();
            //using (var conn = _connectionService.GetConnection())
            //{
            //    conn.Open();
            //    using (var cmd = new NpgsqlCommand("update car_categories set row_status = 1 where id = @id", conn))
            //    {
            //        cmd.Parameters.AddWithValue("id", carCategoryId);
            //        var reader = await cmd.ExecuteNonQueryAsync();
            //    }
            //}
        }

    }
}
