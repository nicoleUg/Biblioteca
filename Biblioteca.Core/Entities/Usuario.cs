using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

public class Usuario : BaseEntity
{
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Rol { get; set; } = "estudiante";
    public bool Activo { get; set; } = true;

    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    public ICollection<Multa> Multas { get; set; } = new List<Multa>();
}