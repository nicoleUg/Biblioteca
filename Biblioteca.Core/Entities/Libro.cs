using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

public class Libro : BaseEntity
{
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Categoria { get; set; } = null!;
    public decimal CostoReposicionBs { get; set; }
    public int TotalEjemplares { get; set; }
    public int EjemplaresDisponibles { get; set; }
    public bool Habilitado { get; set; } = true; // si puede prestarse
    public string CondicionGeneral { get; set; } = "Good"; // New | Good | Fair | Poor

    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
