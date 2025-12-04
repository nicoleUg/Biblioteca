using System.Data;
using Microsoft.Data.SqlClient; 
using Microsoft.Extensions.Configuration;
using Biblioteca.Core.Interfaces;

namespace Biblioteca.Infrastructure.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _cs;

        public DbConnectionFactory(IConfiguration cfg)
        {
           
            _cs = cfg.GetConnectionString("AzureSql")
                  ?? cfg.GetConnectionString("DefaultConnection")
                  ?? "";
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_cs);
        }
    }
}