using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Models
{
  

        public class TestModelDropDownListFor
        {
           
            public int selectedValue { get; set; }
            public IEnumerable<SelectListItem> allValues { get; set; }



            public Annunci.Models.Immobile.Categorie selectedCategoria { get; set; }
            public int selectedCategoriaId { get; set; }
            public IEnumerable<SelectListItem> allCategorie { get; set; }
        }
    }