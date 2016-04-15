using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Models
{
    public class PagedUsers : Shared.Paged
    {
        public IEnumerable<MyUser> Utenti { get; set; }

        public PagedUsers()
        {
            Sort = "MY_LOGIN";
            SortDir = "ASC";
        }


    }
}
