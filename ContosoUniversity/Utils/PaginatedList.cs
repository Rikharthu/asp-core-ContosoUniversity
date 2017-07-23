using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Utils
{
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Current page
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// Page count
        /// </summary>
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            // count item for current page
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            // returns a list containing only the selected page
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

    }
}
