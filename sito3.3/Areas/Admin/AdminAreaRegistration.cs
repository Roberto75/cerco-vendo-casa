using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
              name:  "Admin_default",
              url:  "Admin/{controller}/{action}/{id}",
              defaults:  new { controller = "Admin", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MyWebApplication.Areas.Controllers" }
            );
        }
    }
}