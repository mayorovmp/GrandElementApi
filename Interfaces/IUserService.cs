using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Interfaces
{
    public interface IUserService
    {
        public Task<bool> CheckUser(String login);
    }
}
