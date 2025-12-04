using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.CustomEntities;

/// <summary>
/// Representa una Lista Paginada genérica en el sistema
/// </summary>
/// <remarks>
/// Esta entidad almacena la información principal de la paginación y los elementos de la página actual,
/// y es utilizada para devolver resultados fragmentados en las respuestas de la API.
/// </remarks>
public class PagedList<T> : List<T>
{
    /// <summary>
    /// Número de la página actual
    /// </summary>
    /// <example>1</example>
    public int CurrentPage { get; }

    /// <summary>
    /// Cantidad total de páginas calculadas
    /// </summary>
    /// <example>5</example>
    public int TotalPages { get; }

    /// <summary>
    /// Cantidad de registros por página
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; }

    /// <summary>
    /// Total de registros existentes en la fuente de datos
    /// </summary>
    /// <example>50</example>
    public int TotalCount { get; }

    /// <summary>
    /// Indica si existe una página anterior
    /// </summary>
    /// <example>false</example>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Indica si existe una página siguiente
    /// </summary>
    /// <example>true</example>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Inicializa una nueva instancia de la lista paginada
    /// </summary>
    /// <param name="items">Los elementos de la página actual</param>
    /// <param name="count">El conteo total de elementos</param>
    /// <param name="pageNumber">El número de página actual</param>
    /// <param name="pageSize">El tamaño de la página</param>
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count; PageSize = pageSize; CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    /// <summary>
    /// Crea una lista paginada a partir de una fuente de datos enumerable
    /// </summary>
    /// <param name="source">La fuente de datos original</param>
    /// <param name="page">El número de página deseado</param>
    /// <param name="size">El tamaño de página deseado</param>
    /// <returns>Una nueva instancia de PagedList con los datos paginados</returns>
    public static PagedList<T> Create(IEnumerable<T> source, int page, int size)
    {
        var count = source.Count();
        var items = source.Skip((page - 1) * size).Take(size).ToList();
        return new(items, count, page, size);
    }
}