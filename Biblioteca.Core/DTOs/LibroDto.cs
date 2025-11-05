using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

public class LibroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Autor { get; set; } = "";
    public string Categoria { get; set; } = "General";
    public decimal CostoReposicionBs { get; set; }
    public int TotalEjemplares { get; set; }
    public int EjemplaresDisponibles { get; set; }
    public string CondicionGeneral { get; set; } = "Good";
    public bool Habilitado { get; set; } = true;
}
