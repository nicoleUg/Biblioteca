using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.CustomEntities;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count; PageSize = pageSize; CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }
    public static PagedList<T> Create(IEnumerable<T> source, int page, int size)
    {
        var count = source.Count();
        var items = source.Skip((page - 1) * size).Take(size).ToList();
        return new(items, count, page, size);
    }
}
