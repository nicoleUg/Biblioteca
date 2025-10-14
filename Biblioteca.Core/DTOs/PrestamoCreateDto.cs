using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

public class PrestamoCreateDto
{
    public int UsuarioId { get; set; }
    public int LibroId { get; set; }
    public string FechaPrestamo { get; set; } = null!; // "dd-MM-yyyy"
}