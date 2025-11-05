using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.QueryFilters;

public class LibroQueryFilter : PaginationQueryFilter
{
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public string? Categoria { get; set; }
    public bool? Habilitado { get; set; }
}
