using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;

namespace Biblioteca.Core.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<IEnumerable<Usuario>> GetAllDapperAsync(string? nombre, string? email, string? rol, bool? activo);
        Task<Usuario?> GetByIdDapperAsync(int id);
    }
}