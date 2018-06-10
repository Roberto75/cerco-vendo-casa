using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;
using MyUsers;
using System.Web.Mvc;
using System.Web.Security;

namespace MyWebApplication.Controllers
{
    public class MyBaseController : System.Web.Mvc.Controller
    {
        protected MyManagerCSharp.MySessionData MySessionData;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);


            if (User.Identity.IsAuthenticated && Session != null && Session["MySessionData"] == null)
            {
                FormsAuthentication.SignOut();
            }

            if (Session != null && Session["MySessionData"] != null)
            {
                MySessionData = (Session["MySessionData"] as MyManagerCSharp.MySessionData);
            }
        }



        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
               || filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                // MyHelper.printRequest(Request);

                Debug.WriteLine("MyBaseController.OnAuthorization " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);

                //if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false && (User.Identity.GetType() != typeof(System.Security.Principal.WindowsIdentity)))
                if (Session["MySessionData"] == null || (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false && (User.Identity.GetType() != typeof(System.Security.Principal.WindowsIdentity))))
                {
                    filterContext.Result = new RedirectToRouteResult(
                          new System.Web.Routing.RouteValueDictionary(
                              new
                              {
                                  controller = "Account",
                                  action = "Login",
                                  ReturnUrl = filterContext.HttpContext.Request.RawUrl
                              }));

                }
            }



            //if (!string.IsNullOrEmpty(SimpleSessionPersister.Username))
            //{
            //    filterContext.HttpContext.User = new CustomPrincipal(new CustomIndentity(SimpleSessionPersister.Username));
            //}

           // Debug.WriteLine("IsAuthenticated " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);


            //if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            //{
            //    //  filterContext.HttpContext.User = new MyCustomPrincipal(new MyCustomIndentity() );

            //    // Debug.WriteLine("UserId:" + (User.Identity as MyCustomIndentity).UserId); 

            //}


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



            //            base.OnAuthorization(filterContext);
        }


        protected override void OnException(ExceptionContext filterContext)
        {
            Debug.WriteLine("MyBaseController.OnException");

            if (filterContext.Exception != null)
            {
                Debug.WriteLine("MyBaseController.OnException: " + filterContext.Exception.Message);

                MyManagerCSharp.Log.LogManager log = new MyManagerCSharp.Log.LogManager("log");
                log.mOpenConnection();
                try
                {
                    log.exception("MyBaseController", filterContext.Exception);
                }
                finally
                {
                    log.mCloseConnection();
                }

            }

            if (filterContext.Exception is System.Web.Mvc.HttpAntiForgeryException)
            {
                filterContext.Result = new ViewResult { ViewName = "~/Account/Login" };
            }
            else
            {

                filterContext.Result = new ViewResult { ViewName = "~/Views/Home/Error" };
            }


        }

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);

        //    Debug.WriteLine(String.Format("OnActionExecuting {0} {1}", filterContext.Controller.ToString(), filterContext.ActionDescriptor.ActionName));

        //    Guid? sessionId = null;

        //    string strSessionId = filterContext.HttpContext.Session[MyConstants.MY_SESSION_ACTIVITY_LOG_ID].ToString();

        //    if (!String.IsNullOrEmpty(strSessionId))
        //    {
        //        sessionId = Guid.Parse(strSessionId);
        //    }

        //    string action = filterContext.ActionDescriptor.ActionName;
        //    string controller = filterContext.Controller.ToString();


        //    string param = filterContext.RequestContext.HttpContext.Request.Params.ToString();

        //    string httpMethod = filterContext.RequestContext.HttpContext.Request.HttpMethod;

        //    MyLogActionAsync(sessionId, action, controller, param, httpMethod);
        //}

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            Debug.WriteLine(String.Format("OnActionExecuted {0} {1}", filterContext.Controller.ToString(), filterContext.ActionDescriptor.ActionName));
        }

        private void MyLogActionAsync(Guid? sessionId, string action, string controller, string param, string httpMethod)
        {
            Debug.WriteLine("MyLogActionAsync - START");
            System.Threading.Tasks.Task.Factory.StartNew(() => MyLogAction(sessionId, action, controller, param, httpMethod));
            Debug.WriteLine("MyLogActionAsync - END");
        }

        private void MyLogAction(Guid? sessionId, string action, string controller, string param, string httpMethod)
        {
            Debug.WriteLine("MyLogAction - START");

            if (MySessionData.UserId == -1)
            {
                return;
            }

            MyManagerCSharp.Log.LogUserManager log = new MyManagerCSharp.Log.LogUserManager("DefaultConnection");
            log.mOpenConnection();
            try
            {
                log.insert(MySessionData.UserId, MySessionData.Login, MyManagerCSharp.Log.LogUserManager.LogType.ControllerAction, param, System.Net.IPAddress.Parse(HttpContext.Request.UserHostAddress), controller, action, sessionId, httpMethod);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                log.mCloseConnection();
            }

            //System.Threading.Thread.Sleep(15000);

            Debug.WriteLine("MyLogAction - END");
        }




        protected void MyLogExceptionAsync(string tipo, Exception ex)
        {
            Debug.WriteLine("MyLogExceptionAsync - START");
            System.Threading.Tasks.Task.Factory.StartNew(() => MyLogException(tipo, ex));
            Debug.WriteLine("MyLogExceptionAsync - END");
        }

        private void MyLogException(string tipo, Exception ex)
        {
            Debug.WriteLine("MyLogException - START");
            string currentController = "";
            string currentAction = "";


            if (RouteData != null)
            {
                if (RouteData.Values["controller"] != null && !String.IsNullOrEmpty(RouteData.Values["controller"].ToString()))
                {
                    currentController = RouteData.Values["controller"].ToString();
                }

                if (RouteData.Values["action"] != null && !String.IsNullOrEmpty(RouteData.Values["action"].ToString()))
                {
                    currentAction = RouteData.Values["action"].ToString();
                }
            }

            MyManagerCSharp.Log.LogManager log = new MyManagerCSharp.Log.LogManager("DefaultConnection");
            log.mOpenConnection();
            try
            {
                if (ex.Message.StartsWith("Timeout expired"))
                {
                    //  log.insert("Timeout expired", String.Format("{0}.{1}.{2}", currentController, currentAction, tipo), "", "", MyManagerCSharp.Log.LogManager.Level.Exception);
                    log.insert(tipo, String.Format("{0}.{1}.{2}", currentController, currentAction, tipo), "", "Timeout expired", MyManagerCSharp.Log.LogManager.Level.Exception);
                }
                else
                {
                    log.exception(ex, "", String.Format("{0}.{1}", currentController, currentAction), tipo);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                log.mCloseConnection();
            }

            //System.Threading.Thread.Sleep(15000);

            Debug.WriteLine("MyLogException - END");
        }

        protected void sendMailExceptionAsync(Exception ex)
        {
            Debug.WriteLine("sendMailExceptionAsync - START");
            System.Threading.Tasks.Task.Factory.StartNew(() => sendMailException(ex, ""));
            Debug.WriteLine("sendMailExceptionAsync - END");
        }

        protected void sendMailExceptionAsync(Exception ex, string messaggio)
        {
            Debug.WriteLine("sendMailExceptionAsync - START");
            System.Threading.Tasks.Task.Factory.StartNew(() => sendMailException(ex, messaggio));
            Debug.WriteLine("sendMailExceptionAsync - END");
        }

        private void sendMailException(Exception ex, string messaggio)
        {
            Debug.WriteLine("sendMailException - START");

            string currentController = "";
            string currentAction = "";


            if (RouteData != null)
            {
                if (RouteData.Values["controller"] != null && !String.IsNullOrEmpty(RouteData.Values["controller"].ToString()))
                {
                    currentController = RouteData.Values["controller"].ToString();
                }

                if (RouteData.Values["action"] != null && !String.IsNullOrEmpty(RouteData.Values["action"].ToString()))
                {
                    currentAction = RouteData.Values["action"].ToString();
                }
            }

            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);

            if (String.IsNullOrEmpty(messaggio))
            {
                mail.sendException(ex, String.Format("Controller [{0}] - Action [{1}]", currentController, currentAction));

            }
            else
            {
                mail.sendException(ex, String.Format("Controller [{0}] - Action [{1}] - {2}", currentController, currentAction, messaggio));
            }




            //System.Threading.Thread.Sleep(15000);

            Debug.WriteLine("sendMailException - END");
        }

    }
}