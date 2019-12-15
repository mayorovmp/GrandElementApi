using GrandElementApi.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class UserService : IUserService
    {
        public async Task<bool> CheckUser(string login)
        {
            var connString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();

            // Insert some data
            //await using (var cmd = new NpgsqlCommand("INSERT INTO data (some_field) VALUES (@p)", conn))
            //{
            //    cmd.Parameters.AddWithValue("p", "Hello world");
            //    await cmd.ExecuteNonQueryAsync();
            //}

            // Retrieve all rows
            await using (var cmd = new NpgsqlCommand("select * from users where login = 'max' and pass = '1'", conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                    return true;
                else
                    return false;
                //while (await reader.ReadAsync())
                //    Console.WriteLine(reader.GetString(0));
            }
        }
    }
}
