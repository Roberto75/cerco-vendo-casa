using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;
using MyUsers;

namespace MyWebApplication.Controllers
{
    public class MyBaseController : System.Web.Mvc.Controller
    {
       

        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {

            //if (!string.IsNullOrEmpty(SimpleSessionPersister.Username))
            //{
            //    filterContext.HttpContext.User = new CustomPrincipal(new CustomIndentity(SimpleSessionPersister.Username));
            //}

            Debug.WriteLine("IsAuthenticated " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);


            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
              //  filterContext.HttpContext.User = new MyCustomPrincipal(new MyCustomIndentity() );

               // Debug.WriteLine("UserId:" + (User.Identity as MyCustomIndentity).UserId); 

            }


            //if (System.Web.HttpContext.Current.Session["MySessionData"] != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["MySessionData"].ToString()))
            //{

            //    MyCustomIndentity identity = filterContext.HttpContext.User.Identity as MyCustomIndentity;


            //    // do some stuff here and assign a custom principal:
            //    MyCustomPrincipal principal = new MyCustomPrincipal(identity);
            //    // here you can assign some custom property that every user 
            //    // (even the non-authenticated have)

            //    // set the custom principal
            //    filterContext.HttpContext.User = principal;
            //}



            base.OnAuthorization(filterContext);
        }
    }
}