using GrandElementApi.Data;
using GrandElementApi.Extensions;
using GrandElementApi.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            using var db = new ApplicationContext();
            var user = await db.Users.FirstOrDefaultAsync(u=>u.Login == login && u.Pass == pass);
            if(user == null)
                throw new Exception($"Имя {login} и пароль не найдены.");
            return user;
        }

        public async Task<User> GetUserAsync(Guid session) {
            using var db = new ApplicationContext();
            var s = db.Sessions.Include(s=>s.User).FirstOrDefault(s => s.Token == session);
            // var u = s.User;
            var u = await db.Users.Where(u => u.Sessions.Any(t=>t.Id == s.Id)).FirstAsync();
            if (u == null)
                throw new Exception("Сессия не найдена");
            return u;
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
