using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class AdminController : MyBaseController
    {
        //[MyAuthorizeAttribute(Roles = "Admin, SuperUser")]
        public ActionResult Index()
        {

            Models.ModelHome model = new Models.ModelHome();

          //  System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
           // System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            //model.version = fvi.FileVersion;

            model.versionMyWebApplication = typeof(MyWebApplication.MvcApplication).Assembly.GetName().Version.ToString();
            model.versionImmobiliareCSharp = typeof(Annunci.ImmobiliareManager).Assembly.GetName().Version.ToString();
            model.versionImmobiliareVb = typeof(ImmobiliareVb.RevoAgent).Assembly.GetName().Version.ToString();
            model.versionMyManagerCSharp = typeof(MyManagerCSharp.ManagerDB).Assembly.GetName().Version.ToString();
            model.versionMyUsers = typeof(MyUsers.UserManager).Assembly.GetName().Version.ToString();
            model.versionMVC = typeof(Controller).Assembly.GetName().Version.ToString();

            return View(model);
        }


        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }


       
        public ActionResult MD5(string textinput)
        {
            if (!String.IsNullOrEmpty(textinput))
            {
                ViewBag.valore = textinput;
                ViewBag.risultato = MyManagerCSharp.SecurityManager.getMD5Hash(textinput);
            }
            return View();
        }

    }
}
