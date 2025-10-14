using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

public class PrestamoDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int LibroId { get; set; }
    public string Estado { get; set; } = null!;
    public string FechaPrestamo { get; set; } = null!;
    public string FechaLimite { get; set; } = null!;
    public string? FechaDevolucion { get; set; }
}