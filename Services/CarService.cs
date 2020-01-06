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
        public async Task<List<Car>> AllCarsAsync()
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("select c.id, c.owner, c.state_number, c.contacts, c.comments, cc.id, cc.name " +
                    "from cars c left join car_categories cc on c.car_category_id = cc.id", conn))
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
                            CarCategory = reader.IsDBNull(5) ? null : new CarCategory() { Id = reader.GetInt32(5), Name = reader.GetString(6) }
                        });
                    }
                    return data;
                }
            }
        }
    }
}
