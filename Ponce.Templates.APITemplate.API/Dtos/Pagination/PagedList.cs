using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.API.Helpers;

namespace Ponce.Templates.APITemplate.API.Dtos.Pagination;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, PaginationRequest request)
    {
        var notNulleablePageNumber = request.PageNumber ?? 0;
        var notNulleablePageSize = request.PageSize ?? ApiConstants.MAX_PAGE_SIZE;
        var count = await source.CountAsync();
        // If page number is zero, return all records (no pagination)
        var items = notNulleablePageNumber == 0 ? (await source.ToListAsync()) :
            await (source.Skip((notNulleablePageNumber - 1) * notNulleablePageSize).Take(notNulleablePageSize).ToListAsync());

        return new PagedList<T>(items, count, notNulleablePageNumber, notNulleablePageSize);
    }
}
