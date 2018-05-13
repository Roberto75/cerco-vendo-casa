using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MyWebApplication.Models
{
    public class ModelImmobiliareSearch
    {
        public string Regione { get; set; }
        public string Provincia { get; set; }
        public string Comune { get; set; }
       

        //public ModelRegioneProvinciaComune RegioneProvinciaComune { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<Annunci.Models.Immobile> Immobili { get; set; }

        public int TotalRows { get; set; }
        public int PageSize { get; set; }

        public int TotalPages { get {
            if (PageSize == 0)
            {
                return 0;
            }
            return TotalRows / PageSize; } }
        public bool HasPreviousPage { get { return (PageNumber > 1); } }
        public bool HasNextPage { get { return (PageNumber < TotalPages); } }

        public int Categoria { get; set; }


        //
        // public IEnumerable <Sys SelectListItem

        public IEnumerable<System.Web.Mvc.SelectListItem> testList()
        {
            List<System.Web.Mvc.SelectListItem> risultato = new List<System.Web.Mvc.SelectListItem>() ;

           System.Web.Mvc.SelectListItem i = new System.Web.Mvc.SelectListItem ();
            i.Text = "a";
            i.Value = "b";


            risultato.Add (i);

            return risultato.ToArray ();
        }


        // public Syst
        public IEnumerable<System.Web.Mvc.SelectListItem> Categorie
        {
            get
            {
                return from Annunci.Models.Immobile.Categorie valore in Enum.GetValues(typeof(Annunci.Models.Immobile.Categorie))
                       select new System.Web.Mvc.SelectListItem() { Text = valore.ToString(), Value = ((int)valore).ToString() };



            }

        }
    }
}