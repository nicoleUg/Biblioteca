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
                SELECT m.Id, m.PrestamoId, m.UsuarioId, m.Motivo, m.MontoBs, m.Estado, m.Detalle
                FROM Multa m
                WHERE (@UsuarioId IS NULL OR m.UsuarioId = @UsuarioId)
                  AND (@Estado IS NULL OR m.Estado = @Estado)
                ORDER BY m.Id DESC";

            return await connection.QueryAsync<Multa>(sql, new { UsuarioId = usuarioId, Estado = estado });
        }

        public async Task<Multa?> GetByIdDapperAsync(int id)
        {
            var connection = _context.Database.GetDbConnection();

            var sql = @"
                SELECT m.Id, m.PrestamoId, m.UsuarioId, m.Motivo, m.MontoBs, m.Estado, m.Detalle
                FROM Multa m
                WHERE m.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Multa>(sql, new { Id = id });
        }

        public async Task<bool> TienePendientesPorPrestamoAsync(int prestamoId)
        {
            return await _context.Multas
                .AnyAsync(m => m.PrestamoId == prestamoId && m.Estado == "Pending");
        }
    }
}
