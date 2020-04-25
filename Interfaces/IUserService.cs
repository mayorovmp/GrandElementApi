using GrandElementApi.Data;
using System;
using System.Threading.Tasks;

namespace GrandElementApi.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetUserAsync(string login, string pass);
        public Task<Guid> MakeSessionAsync(int userId);
        public Task<bool> IsValidTokenAsync(string token);
        public Task<User> GetUserAsync(Guid session);
    }
}
