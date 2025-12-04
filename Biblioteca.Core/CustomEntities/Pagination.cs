using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.CustomEntities;

/// <summary>
/// Representa la Metadata de Paginación en el sistema
/// </summary>
/// <remarks>
/// Esta entidad almacena la información principal de los metadatos de paginación
/// y es utilizada para devolver el resumen de navegación en las respuestas de la API.
/// </remarks>
public class Pagination
{
    /// <summary>
    /// Total de registros encontrados en la consulta
    /// </summary>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// Cantidad de registros mostrados por página
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; }

    /// <summary>
    /// Número de la página actual
    /// </summary>
    /// <example>1</example>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Cantidad total de páginas disponibles
    /// </summary>
    /// <example>10</example>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica si existe una página siguiente
    /// </summary>
    /// <example>true</example>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Indica si existe una página anterior
    /// </summary>
    /// <example>false</example>
    public bool HasPreviousPage { get; set; }
}