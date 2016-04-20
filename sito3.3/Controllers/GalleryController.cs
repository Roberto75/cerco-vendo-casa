using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace MyWebApplication.Controllers
{
    public class GalleryController : Controller
    {
        

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Refresh(string externalId)
        {



            return View();
        }

        [HttpPost]
        public JsonResult Delete(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            Debug.WriteLine(String.Format("ExternalId: {0}" , id));

            Annunci.PhotoManager manager = new Annunci.PhotoManager("immobiliare");

            try
            {
                manager.mOpenConnection();
                manager.deletePhoto((long)id, Server.MapPath("~"));
            }
            finally
            {
                manager.mCloseConnection();
            }


            MyWebApplication.Models.JsonMessageModel model = new Models.JsonMessageModel();
            model.esito = "Success";
            model.messaggio = "Operazione conlusa con successo";
            return Json(model);
        }


    }
}
