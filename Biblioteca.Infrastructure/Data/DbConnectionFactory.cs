using System.Data;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Biblioteca.Core.Interfaces;   // <- IMPORTANTE: usa la interfaz de Core

namespace Biblioteca.Infrastructure.Data
{
    public class DbConnectionFactory : IDbConnectionFactory  // <- IMPLEMENTA LA INTERFAZ
    {
        private readonly string _cs;

        public DbConnectionFactory(IConfiguration cfg)
        {
            _cs = cfg.GetConnectionString("MySql") ?? "";
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_cs);
        }
    }
}
