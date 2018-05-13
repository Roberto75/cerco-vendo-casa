using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class MyUserModel
    {
        public MyUsers.Models.MyUser Utente { get; set; }
        public System.Web.Mvc.MultiSelectList  Gruppi { get; set; }
    }
}