using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    public class TestController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Test1()
        {
            return View();
        }

        public ActionResult Test2()
        {
            return View();
        }



        public ActionResult Table()
        {
            return View();
        }


        public ActionResult Panel01()
        {
            return View();
        }


        public ActionResult ArrayObject()
        {
            return View();
        }






        public ActionResult DropDownListFor()
        {
            List<SelectListItem> valuesAll = new List<SelectListItem>
                                 {
                                     new SelectListItem
                                         {
                                             Value = "",
                                             Text = "--please select--",
                                             Selected = false
                                         },
                                     new SelectListItem
                                         {
                                             Value = "1",
                                             Text = "Permission one",
                                             Selected = false
                                         },
                                     new SelectListItem
                                         {
                                             Value = "2",
                                             Text = "Permission two",
                                             Selected = false
                                         }
                                 };
            MyWebApplication.Models.TestModelDropDownListFor model = new Models.TestModelDropDownListFor();

            model.allValues = valuesAll;
            model.selectedValue = 2;




            model.allCategorie = from Annunci.Models.Immobile.Categorie valore in Enum.GetValues(typeof(Annunci.Models.Immobile.Categorie)) select new System.Web.Mvc.SelectListItem() { Text = valore.ToString(), Value = ((int)valore).ToString() };
            model.selectedCategoria = Annunci.Models.Immobile.Categorie.Affitto;
            model.selectedCategoriaId = (int)model.selectedCategoria;

            return View(model);
        }

    }
}
