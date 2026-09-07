using System.Text.Json;
using Ponce.Templates.APITemplate.API.Dtos.Pagination;

namespace Ponce.Templates.APITemplate.API.Helpers;

public static class PaginationHeaderUtil
{
    public static HttpContext AddPaginationHeader<T>(this HttpContext context, PagedList<T> pagedList) where T : class
    {
        var responseHeaders = context.Response.Headers;
        const string headerKey = "X-Pagination";
        if (!responseHeaders.ContainsKey(headerKey))
        {
            responseHeaders.Append(headerKey, JsonSerializer.Serialize(new
            {
                pagedList.TotalPages,
                pagedList.CurrentPage,
                pagedList.PageSize,
                pagedList.TotalCount,
                pagedList.HasPrevious,
                pagedList.HasNext
            }));
        }
        return context;
    }
}
