using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyWebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }


        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception objError;
            objError = Server.GetLastError();
            if (objError != null)
            {
                Debug.WriteLine("Application_Error: " + objError.GetType().Name + " Message:" + objError.Message);

                //switch (objError.GetType().Name)
                //{
                //    case "MyException":
                //        Server.TransferRequest("~/Home/Error?MyError=" + Server.UrlEncode(objError.Message));
                //        break;
                //    case "HttpAntiForgeryException":
                //        Server.TransferRequest("~/Account/Login");
                //        break;
                //}
            }
        }
    }
}
