using Microsoft.EntityFrameworkCore;
using WallsShop.DTO;

namespace WallsShop.Extensions;

public static class PaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        string categoryName = "All Categories")
    {
        pageNumber = pageNumber > 0 ? pageNumber : 1;
        pageSize = pageSize > 0 ? pageSize : 12;

        var totalCount = await source.CountAsync();

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Data = items,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            CategoryName = categoryName
        };
    }
    //public static async Task<PagedResult<T>> ToARENListAsync<T>(
    //    this IQueryable<T> source,
     
    //    string categoryName = "All Categories")
    //{
       

    //    var totalCount = await source.CountAsync();


    //    var items = await source
    //        .ToListAsync();

    //    return new PagedResult<T>
    //    {
    //        Data = items,
         
    //        CategoryName = categoryName
    //    };
    //}
}