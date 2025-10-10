using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.API.Helpers
{
    public class Pages<T> where T : class
    {
        public string Search { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public IEnumerable<T> Registers { get; set; }
        public Pages(IEnumerable<T> registers, int total, int pageIndex, int PageSize, string search)
        {
            Registers = registers;
            Total = total;
            PageIndex = pageIndex;
            Search = search;
        }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling(Total / (double)PageSize);
            }
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
    }
}