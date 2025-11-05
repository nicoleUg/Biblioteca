using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Biblioteca.Core.Interfaces;

public interface IDapperContext
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
    Task<int> ExecuteAsync(string sql, object? param = null);
    Task<T> ExecuteScalarAsync<T>(string sql, object? param = null);

    void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx);
    void ClearAmbientConnection();
}
