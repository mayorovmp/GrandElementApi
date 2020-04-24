using GrandElementApi.Data;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class CarService
    {
        private readonly IConnectionService _connectionService;
        public CarService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task DeleteCar(int carId)
        {
            using (var db = new ApplicationContext())
            {
                var car = await db.Cars.FindAsync(carId);
                if (car == null)
                    throw new Exception("Идентификатор перевозчика не найден");
                car.RowStatus = RowStatus.Removed;
                await db.SaveChangesAsync();
            }
        }
        public async Task<Car> AddCarAsync(Car car) {
            using (var db = new ApplicationContext())
            {
                db.CarCategories.Attach(car.CarCategory);
                db.Cars.Add(car);
                await db.SaveChangesAsync();
                return car;
            }
        }
        public async Task<Car> EditCarAsync(Car car)
        {
            using (var db = new ApplicationContext()) {
                var storedCar = db.Cars.Where(x => x.Id == car.Id).FirstOrDefault();
                if (storedCar == null)
                {
                    throw new Exception("Идентификатор перевозчика не найден.");
                }
                storedCar.Owner = car.Owner;
                storedCar.Unit = car.Unit;
                storedCar.Vat = car.Vat;
                storedCar.Comments = car.Comments;
                storedCar.Contacts = car.Contacts;
                storedCar.CarCategoryId = car.CarCategoryId;
                storedCar.CarCategory = car.CarCategory;
                storedCar.FreightPrice = car.FreightPrice;
                await db.SaveChangesAsync();
            }
            return car;
        }


        public async Task<List<Car>> AllCarsAsync()
        {
            using (var db = new ApplicationContext()) {
                return await db.Cars.Where(x => x.RowStatus == RowStatus.Active).Include(x=>x.CarCategory).ToListAsync();
            }
        }
    }
}
