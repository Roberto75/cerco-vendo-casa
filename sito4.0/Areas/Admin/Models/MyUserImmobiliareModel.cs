using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class MyUserImmobiliareModel 
    {

        public Annunci.Models.MyUser Utente { get; set; }


        public Annunci.Models.Immobile lastAnnuncio { get; set; }

        // public List<string> SourceDisponibili { get; set; }
        //public List<string> StatiDisponibili { get; set; }

        public System.Collections.Hashtable ContaAnnunciByStato { get; set; }
        public System.Collections.Hashtable ContaTrattativeByStato { get; set; }
    }
}