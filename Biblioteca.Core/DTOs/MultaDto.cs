using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs
{
    public class MultaDto
    {
        public int Id { get; set; }                 // Ignorado en POST
        public int PrestamoId { get; set; }
        public int UsuarioId { get; set; }
        public string Motivo { get; set; } = "Retraso";
        public string Detalle { get; set; } = "";
        public decimal MontoBs { get; set; }
        public string Estado { get; set; } = "Pending";  // Pending | Paid
    }
}
