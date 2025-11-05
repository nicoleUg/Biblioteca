using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Biblioteca.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Repositories
{
    public class MultaRepository : BaseRepository<Multa>, IMultaRepository
    {
        public MultaRepository(BibliotecaContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Multa>> GetAllDapperAsync(int? usuarioId, string? estado)
        {
            var connection = _context.Database.GetDbConnection();

            var sql = @"
                SELECT m.Id, m.PrestamoId, m.UsuarioId, m.Monto, m.Estado, m.Detalle, 
                       m.FechaGeneracion, m.FechaPago
                FROM Multa m
                WHERE (@UsuarioId IS NULL OR m.UsuarioId = @UsuarioId)
                  AND (@Estado IS NULL OR m.Estado = @Estado)
                ORDER BY m.FechaGeneracion DESC";

            return await connection.QueryAsync<Multa>(sql, new { UsuarioId = usuarioId, Estado = estado });
        }

        public async Task<Multa?> GetByIdDapperAsync(int id)
        {
            var connection = _context.Database.GetDbConnection();

            var sql = @"
                SELECT m.Id, m.PrestamoId, m.UsuarioId, m.Monto, m.Estado, m.Detalle, 
                       m.FechaGeneracion, m.FechaPago
                FROM Multa m
                WHERE m.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Multa>(sql, new { Id = id });
        }
    }
}