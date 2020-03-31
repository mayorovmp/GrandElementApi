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
    public class CarService
    {
        private readonly IConnectionService _connectionService;
        public CarService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task DeleteCar(int carId)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("update cars set row_status=1 where id = @id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", carId));
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<Car> AddCarAsync(Car car) {
            using (var conn = _connectionService.GetOpenedConnection())
            {
                using (var cmd = new NpgsqlCommand("insert into cars(owner, contacts, comments, car_category_id, freight_price, unit, vat) values (@owner, @contacts, @comments, @category_id, @freight_price, @unit, @vat) RETURNING id", conn))
                {
                    cmd.Parameters.AddRange(new[] {
                        car.Owner == null ? new NpgsqlParameter("owner", DBNull.Value) : new NpgsqlParameter<string>("owner", car.Owner),
                        car.Contacts == null ? new NpgsqlParameter("contacts", DBNull.Value) : new NpgsqlParameter<string>("contacts", car.Contacts),
                        car.Comments == null ? new NpgsqlParameter("comments", DBNull.Value) : new NpgsqlParameter<string>("comments", car.Comments),
                        car.CarCategory == null ? new NpgsqlParameter("category_id", DBNull.Value) : new NpgsqlParameter("category_id", car.CarCategory.Id),
                        car.FreightPrice == null ? new NpgsqlParameter("freight_price", DBNull.Value) : new NpgsqlParameter("freight_price", car.FreightPrice),
                        car.Unit == null ? new NpgsqlParameter("unit", DBNull.Value) : new NpgsqlParameter("unit", car.Unit),
                        car.VAT ? new NpgsqlParameter("vat", DBNull.Value) : new NpgsqlParameter("vat", car.VAT ? 1 : 0),
                    });

                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        car.Id = reader.GetInt32(0);
                    }
                    return car;
                }
            }
        }
        public async Task<Car> EditCarAsync(Car car)
        {

            if (!car.Id.HasValue)
                throw new ArgumentException("Идентификатор записи пустой");
            int id = car.Id.GetValueOrDefault();

            if (car.Owner == null)
                throw new ArgumentException("Укажите владельца");


            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
update cars set owner = @owner, contacts=@contacts, comments=@comments, car_category_id=@category_id, freight_price=@freight_price, 
unit=@unit,
vat=@vat
where id=@id 
RETURNING id, owner, state_number, contacts, comments, freight_price", conn))
                {
                    cmd.Parameters.AddRange(new[] {
                        new NpgsqlParameter<int>("id", id),
                        new NpgsqlParameter<string>("owner", car.Owner),
                        new NpgsqlParameter<string>("contacts", car.Contacts),
                        new NpgsqlParameter<string>("comments", car.Comments),
                        car.FreightPrice == null ? new NpgsqlParameter("freight_price", DBNull.Value) : new NpgsqlParameter("freight_price", car.FreightPrice),
                        car.CarCategory == null ? new NpgsqlParameter("category_id", DBNull.Value) : new NpgsqlParameter("category_id", car.CarCategory.Id),
                        car.Unit == null ? new NpgsqlParameter("unit", DBNull.Value) : new NpgsqlParameter("unit", car.Unit),
                        car.VAT ? new NpgsqlParameter("vat", DBNull.Value) : new NpgsqlParameter("vat", car.VAT ? 1 : 0),
                    });

                    var reader = await cmd.ExecuteReaderAsync();

                    return await GetCarAsync(id);
                }
            }
        }

        public async Task<Car> GetCarAsync(int id)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select c.id, c.owner, c.contacts, c.comments, cc.id, cc.name, c.freight_price, c.vat, c.unit
from cars c left join car_categories cc on c.car_category_id = cc.id
where c.id = @id and c.row_status=0
order by c.created desc
", conn))
                {
                    cmd.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Integer, id);
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new Car()
                        {
                            Id = reader.GetInt32(0),
                            Owner = reader.SafeGetString(1),
                            Contacts = reader.SafeGetString(2),
                            Comments = reader.SafeGetString(3),
                            CarCategory = reader.IsDBNull(4) ? null : new CarCategory() { Id = reader.GetInt32(4), Name = reader.SafeGetString(5) },
                            FreightPrice = reader.SafeGetDecimal(6),
                            VAT = reader.SafeGetDecimal(7).GetValueOrDefault(1) == 1 ? true : false,
                            Unit = reader.SafeGetString(8)
                        };
                    }
                    else {
                        throw new ArgumentException("Идентификатор не найден");
                    }
                }
            }
        }

        public async Task<List<Car>> AllCarsAsync()
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
select c.id, c.owner, c.contacts, c.comments, cc.id, cc.name, c.freight_price, c.vat, c.unit
from cars c left join car_categories cc on c.car_category_id = cc.id where c.row_status=0
order by c.owner", conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    var data = new List<Car>();
                    while (reader.Read())
                    {
                        data.Add(new Car()
                        {
                            Id = reader.GetInt32(0),
                            Owner = reader.SafeGetString(1),
                            Contacts = reader.SafeGetString(2),
                            Comments = reader.SafeGetString(3),
                            CarCategory = reader.IsDBNull(4) ? null : new CarCategory() { Id = reader.GetInt32(4), Name = reader.SafeGetString(5) },
                            FreightPrice = reader.SafeGetDecimal(6),
                            VAT = reader.SafeGetDecimal(7).GetValueOrDefault(1) == 1 ? true : false,
                            Unit = reader.SafeGetString(8)
                        }) ;
                    }
                    return data;
                }
            }
        }
    }
}
