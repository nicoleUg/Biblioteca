using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Biblioteca.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Repositories
{
    public class PrestamoRepository : BaseRepository<Prestamo>, IPrestamoRepository
    {
        public PrestamoRepository(BibliotecaContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Prestamo>> GetAllDapperAsync(int? usuarioId, int? libroId, string? estado)
        {
            var connection = _context.Database.GetDbConnection();

            var sql = @"
                SELECT p.Id, p.UsuarioId, p.LibroId, p.FechaPrestamo, p.FechaLimite, 
                       p.FechaDevolucion, p.Estado
                FROM Prestamo p
                WHERE (@UsuarioId IS NULL OR p.UsuarioId = @UsuarioId)
                  AND (@LibroId IS NULL OR p.LibroId = @LibroId)
                  AND (@Estado IS NULL OR p.Estado = @Estado)
                ORDER BY p.FechaPrestamo DESC";

            return await connection.QueryAsync<Prestamo>(sql,
                new { UsuarioId = usuarioId, LibroId = libroId, Estado = estado });
        }

        public async Task<Prestamo?> GetByIdDapperAsync(int id)
        {
            var connection = _context.Database.GetDbConnection();

            var sql = @"
                SELECT p.Id, p.UsuarioId, p.LibroId, p.FechaPrestamo, p.FechaLimite, 
                       p.FechaDevolucion, p.Estado
                FROM Prestamo p
                WHERE p.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Prestamo>(sql, new { Id = id });
        }
    }
}