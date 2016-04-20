using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using MyUsers;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class MyBaseController : System.Web.Mvc.Controller
    {





        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {

            Debug.WriteLine("IsAuthenticated " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);

            //            string role= "Admin";
            //          Debug.WriteLine(String.Format ("IsInRole({0}) : {1}", role, System.Web.HttpContext.Current.User.IsInRole(role)));


            //if (!string.IsNullOrEmpty(SimpleSessionPersister.Username))
            //{
            //    filterContext.HttpContext.User = new CustomPrincipal(new CustomIndentity(SimpleSessionPersister.Username));
            //}

            Debug.WriteLine("IsAuthenticated " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);


            //if ( System.Web.HttpContext.Current.Session["MySessionData"] != null && !string.IsNullOrEmpty ( System.Web.HttpContext.Current.Session["MySessionData"].ToString() )) {

            //    MyCustomIndentity identity = filterContext.HttpContext.User.Identity as MyCustomIndentity;


            //// do some stuff here and assign a custom principal:
            //    MyCustomPrincipal principal = new MyCustomPrincipal(identity);
            //// here you can assign some custom property that every user 
            //// (even the non-authenticated have)

            //// set the custom principal
            //filterContext.HttpContext.User = principal;
            //}


            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false)
            {
                //filterContext.Result = new RedirectToRouteResult(
                //    new System.Web.Routing.RouteValueDictionary 
                //    { 
                //            { "language", filterContext.RouteData.Values[ "language" ] }, 
                //            { "controller", "Account" }, 
                //            { "action", "Login" }, 
                //            { "ReturnUrl", filterContext.HttpContext.Request.RawUrl } 
                //    });

                filterContext.Result = new RedirectToRouteResult(
                   new System.Web.Routing.RouteValueDictionary(
                       new
                       {
                           area = "Admin",
                           controller = "Account",
                           action = "Login",
                           ReturnUrl = filterContext.HttpContext.Request.RawUrl
                       }));
            }

            //base.OnAuthorization(filterContext);

            //if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //        new System.Web.Routing.RouteValueDictionary 
            //        { 
            //                { "language", filterContext.RouteData.Values[ "language" ] }, 
            //                { "controller", "Account" }, 
            //                { "action", "Login" }, 
            //                { "ReturnUrl", filterContext.HttpContext.Request.RawUrl } 
            //        });
            //}

        }



    }
}