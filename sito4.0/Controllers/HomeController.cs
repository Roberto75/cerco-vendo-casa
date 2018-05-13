using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace MyWebApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Contatti()
        {
            return View();
        }


        public ActionResult Donazione()
        {

            ViewBag.Description = "New Description";
            ViewBag.Keywords = "AA,BB";
            return View();
        }


        public ActionResult Ok()
        {
            Debug.WriteLine("UrlReferrer {0}", Request.UrlReferrer);

            string messaggio = "";

            if (Request.UrlReferrer.ToString().Contains("/Register"))
            {
                messaggio ="<p> Riceverai una <b>email</b> con una password generata casualmente.</p>";
                //<% If System.Configuration.ConfigurationManager.AppSettings("utenti.password.canSet").ToLower() = "false" Then%>
                messaggio += "<p>Una volta in possesso di queste informazioni potrai accedere al sistema e cambiare la password in una che ricorderai meglio.</p>";
                messaggio += "<p>Grazie per esserti iscritto!</p>";
            }


            ViewData["messaggio"] = messaggio;

            return View();
        }

        public ActionResult Error()
        {
            Debug.WriteLine("UrlReferrer {0}", Request.UrlReferrer);
            Debug.WriteLine("Error {0}", Request["MyError"]);

            return View();
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }
    }
}
