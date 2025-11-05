using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.QueryFilters
{
    public class PrestamoQueryFilter : PaginationQueryFilter
    {
        public int? UsuarioId { get; set; }
        public int? LibroId { get; set; }
        public string? Estado { get; set; }   // "Prestado" | "Devuelto" | ...
    }
}
