using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;

namespace Biblioteca.Core.Interfaces
{
    public interface IPrestamoRepository : IBaseRepository<Prestamo>
    {
        Task<IEnumerable<Prestamo>> GetAllDapperAsync(int? usuarioId, int? libroId, string? estado);
        Task<Prestamo?> GetByIdDapperAsync(int id);
    }
}
