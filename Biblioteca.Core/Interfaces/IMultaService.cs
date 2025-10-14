using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Interfaces;
public interface IMultaService
{
    Task<bool> PagarMultaAsync(int multaId);
    Task<bool> CancelarMultaAsync(int multaId, string? motivo = null);
}
