using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

public class Prestamo : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int LibroId { get; set; }
    public Libro? Libro { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaLimite { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string Estado { get; set; } = "Prestado"; // Prestado, Devuelto, cerrado
    public ICollection<Multa> Multas { get; set; } = new List<Multa>();
}