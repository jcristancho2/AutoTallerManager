using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Shared.Helpers
{
    public class Pages<T> where T : class
    {
        public string Search { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int Total { get; private set; }
        public IEnumerable<T> Registers { get; private set; }
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