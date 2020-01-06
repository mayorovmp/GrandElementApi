using GrandElementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetUserAsync(string login, string pass);
        public Task<Guid> MakeSessionAsync(int userId);
        public Task<bool> IsValidTokenAsync(string token);
    }
}
