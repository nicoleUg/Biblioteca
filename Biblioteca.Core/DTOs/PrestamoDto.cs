using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

public class PrestamoDto
{
    public int Id { get; set; }                     // Ignorado en POST
    public int UsuarioId { get; set; }
    public int LibroId { get; set; }
    public string Estado { get; set; } = "Prestado";   // Servidor lo fija en POST
    public string FechaPrestamo { get; set; } = "";    // dd-MM-yyyy (requerido en POST)
    public string FechaLimite { get; set; } = "";      // Servidor lo calcula en POST
    public string? FechaDevolucion { get; set; }       // Solo en devolución
}