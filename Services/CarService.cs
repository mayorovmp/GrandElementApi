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
                using (var cmd = new NpgsqlCommand("insert into cars(owner, state_number, contacts, comments, car_category_id) values (@owner, @number, @contacts, @comments, @category_id) RETURNING id", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<string>("owner", car.Owner));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("number", car.StateNumber));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("contacts", car.Contacts));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("comments", car.Comments));
                    if (car.CarCategory == null)
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("category_id", DBNull.Value));
                    }
                    else
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("category_id", car.CarCategory.Id));
                    }
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
update cars set owner = @owner, state_number=@number, contacts=@contacts, comments=@comments, car_category_id=@category_id 
where id=@id 
RETURNING id, owner, state_number, contacts, comments", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter<int>("id", id));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("owner", car.Owner));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("number", car.StateNumber));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("contacts", car.Contacts));
                    cmd.Parameters.Add(new NpgsqlParameter<string>("comments", car.Comments));
                    if (car.CarCategory == null)
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("category_id", DBNull.Value));
                    }
                    else
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("category_id", car.CarCategory.Id));
                    }
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
select c.id, c.owner, c.state_number, c.contacts, c.comments, cc.id, cc.name 
from cars c left join car_categories cc on c.car_category_id = cc.id
where c.id = @id and c.row_status=0", conn))
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
                            StateNumber = reader.SafeGetString(2),
                            Contacts = reader.SafeGetString(3),
                            Comments = reader.SafeGetString(4),
                            CarCategory = reader.IsDBNull(5) ? null : new CarCategory() { Id = reader.GetInt32(5), Name = reader.SafeGetString(6) }
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
                using (var cmd = new NpgsqlCommand("select c.id, c.owner, c.state_number, c.contacts, c.comments, cc.id, cc.name " +
                    "from cars c left join car_categories cc on c.car_category_id = cc.id where c.row_status=0", conn))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    var data = new List<Car>();
                    while (reader.Read())
                    {
                        data.Add(new Car() { 
                            Id = reader.GetInt32(0), 
                            Owner = reader.SafeGetString(1),
                            StateNumber = reader.SafeGetString(2),
                            Contacts = reader.SafeGetString(3),
                            Comments = reader.SafeGetString(4),
                            CarCategory = reader.IsDBNull(5) ? null : new CarCategory() { Id = reader.GetInt32(5), Name = reader.SafeGetString(6) }
                        });
                    }
                    return data;
                }
            }
        }
    }
}
