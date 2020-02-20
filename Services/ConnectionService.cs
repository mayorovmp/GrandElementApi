using GrandElementApi.Interfaces;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

namespace GrandElementApi.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly string _connectionString;

        public ConnectionService()
        {
            _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        public NpgsqlConnection GetConnection() {
            return new NpgsqlConnection(_connectionString);
        }

        public NpgsqlConnection GetOpenedConnection()
        {
            var conn = GetConnection();
            conn.Open();
            return conn;
        }
    }
}
