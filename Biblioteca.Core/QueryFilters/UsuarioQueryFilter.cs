using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.QueryFilters
{
    public class UsuarioQueryFilter : PaginationQueryFilter
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; }   // "estudiante" | "staff"
        public bool? Activo { get; set; }
    }
}
