using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class MyGroupModel
    {
        public MyUsers.Models.MyGroup Gruppo { get; set; }
        public IEnumerable<MyUsers.Models.MyUser> Utenti { get; set; }

        public System.Web.Mvc.MultiSelectList Ruoli { get; set; }
    }
}