using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;

namespace Biblioteca.Core.Interfaces
{
    public interface IMultaRepository : IBaseRepository<Multa>
    {
        Task<IEnumerable<Multa>> GetAllDapperAsync(int? usuarioId, string? estado);
        Task<Multa?> GetByIdDapperAsync(int id);
    }
}
