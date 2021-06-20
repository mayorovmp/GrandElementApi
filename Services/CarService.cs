using GrandElementApi.Data;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
                db.Cars.Add(car);
                await db.SaveChangesAsync();
                return car;
            }
        }
        public async Task<Car> EditCarAsync(Car car)
        {
            using (var db = new ApplicationContext()) {
                var storedCar = db.Cars.Where(x => x.Id == car.Id)
                    .Include(x => x.CarNumbers).FirstOrDefault();
                if (storedCar == null)
                {
                    throw new Exception("Идентификатор перевозчика не найден.");
                }
                storedCar.Owner = car.Owner;
                storedCar.Vat = car.Vat;
                storedCar.Comments = car.Comments;
                storedCar.Contacts = car.Contacts;
                storedCar.CarNumbers.RemoveAll(cn => !car.CarNumbers.Contains(cn));
                storedCar.CarNumbers.AddRange(car.CarNumbers.FindAll(cn => !storedCar.CarNumbers.Contains(cn)));
                await db.SaveChangesAsync();
            }
            return car;
        }
        public async Task<List<Car>> Favorite(int lastDays, int limit)
        {
            using var db = new ApplicationContext();
            var carsTask = db.Cars
                .Where(c => c.Requests.Any(r => r.DeliveryStart >= DateTime.Now.AddDays(-lastDays)) && c.RowStatus == RowStatus.Active)
                .OrderByDescending(c => c.Requests.Count(r => r.DeliveryStart >= DateTime.Now.AddDays(-lastDays)))
                .Take(limit)
                .Include(c => c.CarNumbers)
                .ToListAsync();
            return await carsTask;
        }
        public async Task<List<Car>> AllCarsAsync()
        {
            using (var db = new ApplicationContext())
            {
                return await db.Cars.Where(x => x.RowStatus == RowStatus.Active)
                    .Include(x => x.CarNumbers)
                    .ToListAsync();
            }
        }
        public async Task<List<Car>> Search(string name, int limit, int offset)
        {
            using (var db = new ApplicationContext())
            {
                return await db.Cars
                    .Where(x => x.Owner.ToLower().Contains(name.ToLower()) && x.RowStatus == RowStatus.Active)
                    .OrderBy(x => x.Owner)
                    .Skip(offset)
                    .Take(limit)
                    .Include(x => x.CarNumbers)
                    .ToListAsync();
            }
        }
    }
}
