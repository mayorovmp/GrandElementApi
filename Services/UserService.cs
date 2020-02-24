using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GrandElementApi.Services
{
    public class UserService : IUserService
    {
        IConnectionService _connectionService;
        public UserService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<User> GetUserAsync(string login, string pass)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("select id, login, pass, name from users where login = @login and pass = @pass", conn))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    cmd.Parameters.AddWithValue("pass", pass);
                    var reader = await cmd.ExecuteReaderAsync();

                    if (!reader.HasRows)
                        throw new ArgumentException($"Имя {login} и пароль не найдены.");
                    else {
                        reader.Read();
                        return new User() {Id = reader.GetInt32(0), Login = reader.SafeGetString(1), Password=reader.SafeGetString(2), Name=reader.SafeGetString(3) };
                    }
                }
            }
        }

        public async Task<bool> IsValidTokenAsync(string token)
        {
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("select * from sessions where token = @token", conn))
                {
                    cmd.Parameters.AddWithValue("token", token);
                    var c = await cmd.ExecuteScalarAsync();
                    if (c == null)
                        return false;
                    return true;
                }
            }
        }

        public async Task<Guid> MakeSessionAsync(int userId)
        {
            var guid = Guid.NewGuid();
            using (var conn = _connectionService.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO sessions (user_id, login_date, token) VALUES (@user_id, @login_date, @guid)", conn))
                {
                    cmd.Parameters.AddWithValue("user_id", userId);
                    cmd.Parameters.AddWithValue("login_date", DateTime.Now);
                    cmd.Parameters.AddWithValue("guid", guid);

                    await cmd.ExecuteNonQueryAsync();
                    return guid;
                }
            }
        }
    }
}
