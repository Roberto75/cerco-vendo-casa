using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MyWebApplication.Models;
using System.Web.Security;
using System.Diagnostics;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class AccountController : MyBaseController
    {
        private MyUsers.UserManager _manager = new MyUsers.UserManager("utenti");

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LogOnModel model = new LogOnModel();
            model.Password = "";
#if DEBUG
            model.UserName = "roberto.rutigliano";
            model.Password = "r0b3rt0";
#endif
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            //  if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            if (ModelState.IsValid)
            {
                string messaggioDiErrore;

                // MyUsers.UserManager manager = new UserManager("utenti");
                long userId;

                _manager.mOpenConnection();

                try
                {
                    userId = _manager.isAuthenticated(model.UserName.Trim(), model.Password.Trim());

                    if (userId != -1)
                    {

                        string profiloId = "";
                        //profiloId = manager.getProfilo(userId);



                        string userDataString;
                        userDataString = userId + ";" + model.UserName.Trim() + ";" + profiloId.ToUpper() + ";";

                        HttpCookie authCookie = FormsAuthentication.GetAuthCookie(model.UserName, model.RememberMe);
                        //Get the FormsAuthenticationTicket out of the encrypted cookie
                        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                        //Create a new FormsAuthenticationTicket that includes our custom User Data
                        FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userDataString);

                        //Update the authCookie's Value to use the encrypted version of newTicket
                        authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                        //Manually add the authCookie to the Cookies collection

                        Response.Cookies.Add(authCookie);

                        string temp;
                        temp = FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe);

                        Debug.WriteLine("FormsAuthentication.GetRedirectUrl " + temp);


                        /** SESSIONE **/
                        MyManagerCSharp.MySessionData session = new MyManagerCSharp.MySessionData(userId);
                        session.Login = model.UserName.Trim();
                        //session.Login = _manager.getLogin(userId);
                        //   session.Roles = manager.getRoles(userId);
                        // session.Profili = manager.getProfili(userId);
                        // session.Groups = manager.getGroupSmall(userId);

                        Session["MySessionData"] = session;

                        //System.Web.HttpContext.Current.User = new MyUsers.MyCustomPrincipal(new MyUsers.MyCustomIndentity(userId, model.UserName));
                        // System.Web.HttpContext.Current.
                        //System.Threading.Thread.CurrentPrincipal = new MyUsers.MyCustomPrincipal(new MyUsers.MyCustomIndentity(userId, model.UserName));
                        //System.Web.HttpContext.Current.Session["MySessionData"] = userId;

                        // Debug.WriteLine("UserId:" + (User.Identity as MyCustomIndentity).UserId); 
                    }


                }
                catch (MyManagerCSharp.MyException ex)
                {
                    if (ex.ErrorCode == MyManagerCSharp.MyException.ErrorNumber.LoginPasswordErrati)
                    {
                        messaggioDiErrore = ex.Message;
                    }
                    else if (ex.ErrorCode == MyManagerCSharp.MyException.ErrorNumber.UtenteDisabilitato)
                    {
                        messaggioDiErrore = ex.Message;
                    }
                    else
                    {
                        //errore non gestito!!
                        messaggioDiErrore = "Errore durante la procedura di login. Contattare l'amministratore di sistema.";
                        MyManagerCSharp.MailManager.send(ex);
                    }

                    //sessionData.setJavaScriptMessage(messaggioDiErrore)
                    //If Page.AppRelativeVirtualPath = "~/utenti/notAuthenticated.aspx" Then
                    //    redirectTo = "~/utenti/notAuthenticated.aspx"
                    //ElseIf Page.AppRelativeVirtualPath = "~/admin/login.aspx" Then
                    //    redirectTo = "~/admin/login.aspx"
                    //Else
                    //    redirectTo = "~/utenti/login.aspx"
                    //End If

                    //Response.Redirect(redirectTo)

                    ModelState.AddModelError("", messaggioDiErrore);
                    return View(model);
                }
                finally
                {
                    _manager.mCloseConnection();
                }

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }



        [AllowAnonymous]
        public ActionResult LogOff()
        {
            TempData["AREA"] = "Admin";
            //WebSecurity.Logout();
            FormsAuthentication.SignOut();

            if (MySessionData != null)
            {
                MySessionData.LogOff();
            }

            //            return RedirectToAction("Index", "Admin");
            return RedirectToAction("Login", "Account");
        }


    }
}
