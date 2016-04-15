using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Models
{
    public class SearchUsers : PagedUsers
    {
        public MyUser filter = new MyUser();
        public int tipo;
    }
}
