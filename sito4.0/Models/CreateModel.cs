using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Models
{
    public class CreateModel
    {
        public Annunci.Models.Immobile immobile { get; set; }

     
        public List<MyManagerCSharp.Models.MyItem> comboRegioni { get; set; }
        public List<MyManagerCSharp.Models.MyItem> comboProvince { get; set; }
        public List<MyManagerCSharp.Models.MyItem> comboComuni { get; set; }

        public CreateModel()
        {
            comboProvince = new List<MyManagerCSharp.Models.MyItem>();
            comboComuni = new List<MyManagerCSharp.Models.MyItem>();
        }
    }
}