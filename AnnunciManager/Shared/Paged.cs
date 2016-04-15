using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Shared
{
    public class Paged
    {
        public int PageNumber { get; set; }
        public int TotalRows { get; set; }
        public int PageSize { get; set; }

        public int TotalPages
        {
            get
            {
                if (PageSize == 0)
                {
                    return 0;
                }

                return (TotalRows / PageSize) + 1;
            }
        }
        public bool HasPreviousPage { get { return (PageNumber > 1); } }
        public bool HasNextPage { get { return (PageNumber < TotalPages); } }

        public string Sort { get; set; }
        public string SortDir { get; set; }

        public Paged()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
    }
}


