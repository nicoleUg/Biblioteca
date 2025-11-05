using Biblioteca.Core.Entities;
using System.Data;

namespace Biblioteca.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositorios base genéricos
        IBaseRepository<Libro> Libros { get; }
        IBaseRepository<Usuario> Usuarios { get; }
        IBaseRepository<Prestamo> Prestamos { get; }
        IBaseRepository<Multa> Multas { get; }

        // Repositorios extendidos (con métodos específicos)
        ILibroRepository LibrosEx { get; }
        IUsuarioRepository UsuariosEx { get; }
        IPrestamoRepository PrestamosEx { get; }
        IMultaRepository MultasEx { get; }

        // Métodos de transacción
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();

        // Métodos para obtener conexión/transacción (para Dapper)
        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();
    }
}