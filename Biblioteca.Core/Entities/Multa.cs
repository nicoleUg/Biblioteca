using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

public class Multa : BaseEntity
{
    public int PrestamoId { get; set; }
    public Prestamo? Prestamo { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string Motivo { get; set; } = null!;
    public string? Detalle { get; set; }
    public decimal MontoBs { get; set; }
    public string Estado { get; set; } = "Pending"; // Pending, Paid, canceled
}