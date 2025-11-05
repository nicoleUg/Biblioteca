using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;

namespace Biblioteca.Core.Interfaces
{

    public interface ILibroRepository : IBaseRepository<Libro>
    {
        Task<IEnumerable<Libro>> GetAllDapperAsync(
            string? titulo,
            string? autor,
            string? categoria,
            bool? habilitado);
        Task<Libro?> GetByIdDapperAsync(int id);
    }
}
