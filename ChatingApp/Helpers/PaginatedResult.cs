using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Helpers
{
    public class PaginatedResult<T>
    {
        public PaginationMetadata metadata { get; set; } = default!;
        public List<T> Items { get; set; } = [];
    }

    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class PaginationHelper
    {
        public static async Task<PaginatedResult<T>> createPaginationAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedResult<T>
            {
                metadata = new PaginationMetadata
                {
                    CurrentPage = pageNumber,
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    TotalPages = totalPages
                },
                Items = items
            };
        }
    }
}
