using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Interfaces
{
    public interface IConnectionService
    {
        public NpgsqlConnection GetConnection();
    }
}
