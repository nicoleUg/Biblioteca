using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;
using Biblioteca.Core.Interfaces;

namespace Biblioteca.Infrastructure.Data
{
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnectionFactory _factory;
        private static readonly AsyncLocal<(IDbConnection? Conn, IDbTransaction? Tx)> _ambient = new();

        public DapperContext(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx) => _ambient.Value = (conn, tx);
        public void ClearAmbientConnection() => _ambient.Value = (null, null);

        private (IDbConnection conn, IDbTransaction? tx, bool owns) GetConn()
        {
            var a = _ambient.Value;
            if (a.Conn != null) return (a.Conn, a.Tx, false);
            return (_factory.CreateConnection(), null, true);
        }

        private static async Task OpenIfNeededAsync(IDbConnection c)
        {
            if (c is DbConnection dc && dc.State == ConnectionState.Closed)
                await dc.OpenAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            var (c, tx, owns) = GetConn();
            try { await OpenIfNeededAsync(c); return await c.QueryAsync<T>(sql, param, tx); }
            finally { if (owns) { if (c is DbConnection dc) await dc.CloseAsync(); c.Dispose(); } }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            var (c, tx, owns) = GetConn();
            try { await OpenIfNeededAsync(c); return await c.QueryFirstOrDefaultAsync<T>(sql, param, tx); }
            finally { if (owns) { if (c is DbConnection dc) await dc.CloseAsync(); c.Dispose(); } }
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            var (c, tx, owns) = GetConn();
            try { await OpenIfNeededAsync(c); return await c.ExecuteAsync(sql, param, tx); }
            finally { if (owns) { if (c is DbConnection dc) await dc.CloseAsync(); c.Dispose(); } }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null)
        {
            var (c, tx, owns) = GetConn();
            try
            {
                await OpenIfNeededAsync(c);
                var res = await c.ExecuteScalarAsync(sql, param, tx);
                return res is null || res == DBNull.Value ? default! : (T)System.Convert.ChangeType(res, typeof(T));
            }
            finally { if (owns) { if (c is DbConnection dc) await dc.CloseAsync(); c.Dispose(); } }
        }
    }
}
