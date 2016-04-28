using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyWebApplication.Models;
using MyManagerCSharp;
using System.Diagnostics;
using MyUsers;

namespace MyWebApplication.Controllers
{

    public class AccountController : MyBaseController
    {
        public const bool MY_CUSTOM_IDENTITY = true;

        private string testoPrivacy = "Informativa ai sensi dell’art. 13 D.Lgs. 196/03 <br />" + Environment.NewLine +
              "Ai sensi dell'art. 13 D.Lgs. 196/03 ti informiamo che i dati da te forniti verranno trattati nel rispetto della normativa vigente e degli obblighi di riservatezza. <br />" + Environment.NewLine +
              "I dati saranno conservati e protetti nel nostro archivio informatico e saranno utilizzati per le seguenti finalità: <br />" + Environment.NewLine +
              "<ol>" +
              "<li>Raccolta e conservazione dei dati personali da parte di " + System.Configuration.ConfigurationManager.AppSettings["application.name"] + ", al fine di adempiere agli oneri connessi ai servizi legati al sito " + System.Configuration.ConfigurationManager.AppSettings["application.url"] + "</li>" + Environment.NewLine +
              "<li>Elaborazione dei dati personali da parte di " + System.Configuration.ConfigurationManager.AppSettings["application.name"] + " per definire il profilo dell'utente; " + "</li>" + Environment.NewLine +
              "<li>Utilizzo dei profili elaborati da " + System.Configuration.ConfigurationManager.AppSettings["application.name"] + " per finalità di marketing, iniziative promozionali, offerta diretta di beni e di servizi, ricerche di mercato;  " + "</li>" + Environment.NewLine +
              "</ol>" +
              "Ti informiamo che, ai sensi dell'art. 7 del D.Lgs. 157/1995, hai il diritto di conoscere, aggiornare, bloccare, cancellare i tuoi dati o opporti all'utilizzo degli stessi, se trattati in violazione di legge. Potrai inoltre avere tutte le informazioni sulle generalità del Responsabile (se nominato) del trattamento dei dati. <br />" + Environment.NewLine +
              "Il conferimento dei tuoi dati personali é facoltativo. L'eventuale rifiuto a conferire detti dati può comportare la mancata registrazione.  <br />" + Environment.NewLine +
              "Dopo aver letto la presente informativa, selezionando la casella \"Accetto\" qui sotto manifesti il tuo libero e incondizionato consenso al trattamento dei tuoi dati personali da parte di " + System.Configuration.ConfigurationManager.AppSettings["application.name"] + " per le suindicate finalità, comunque connesse o strumentali alle proprie attività.";


        private int _maxWidth = 80;
        private int _maxHeight = 80;

        private MyUsers.UserManager _manager = new MyUsers.UserManager("utenti");


        // This action handles the form POST and the upload
        [Authorize]
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the fielname
                //var fileName = System.IO.Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                string path;
                path = "~/Public/UserFiles/" + (User.Identity as MyUsers.MyCustomIdentity).Login;

                path = Server.MapPath(path);

                if (!System.IO.Directory.Exists(path))
                {
                    //La cartella non esiste creala
                    //Try
                    System.IO.Directory.CreateDirectory(path);
                    //Catch ex As Exception
                    //    'Non è stato possibile creare la cartella
                    //    MyManager.MailManager.send(ex)
                    //    Exit Sub
                    //End Try
                    //
                }

                path = path + "/photo.gif";

                //Cambio nome al file 

                System.Drawing.Image myImage = null;

                try
                {

                    myImage = System.Drawing.Image.FromStream(file.InputStream);
                }
                catch (Exception e)
                {
                    //NON si tratta di un'immagine!
                    return RedirectToAction("Error", "Home");
                }


                if (myImage.Height > _maxHeight)
                {
                    myImage = Annunci.PhotoManager.resizeImageHeight(myImage, _maxHeight);
                }

                if (myImage.Width > _maxWidth)
                {
                    myImage = Annunci.PhotoManager.resizeImageWidth(myImage, _maxWidth);
                }

                myImage.Save(path, System.Drawing.Imaging.ImageFormat.Gif);




                //   file.SaveAs(Server.MapPath( pathImage));
            }
            // redirect back to the index action to show the form once again
            return RedirectToAction("Manage");
        }


        [Authorize]
        public ActionResult Manage(UserProfile model)
        {
            _manager.mOpenConnection();

            MyUsers.Models.MyUser u = null;
            try
            {
                u = _manager.getUser(MySessionData.UserId);

                if (u == null)
                {
                    return RedirectToAction("Error", "Home");
                }


                model.login = u.login;

                if (u.customerId != null)
                {
                    model.customerId = (long)u.customerId;
                }
                else
                {
                    model.customerId = -1;
                }
                
                string pathImage;
                pathImage = "~/public/UserFiles/" + u.login + "/photo.gif";


                if (!System.IO.File.Exists(Server.MapPath(pathImage)))
                {

                    if (u.customerId == 0)
                    {
                        pathImage = "~/Images/immobiliare/_privato.gif";
                    }
                    else
                    {
                        pathImage = "~/Images/immobiliare/_agenzia.gif";
                    }
                }

                //29/08/2013 per la cache
                pathImage += "?" + DateTime.Now.Ticks;
                model.pathImageProfile = pathImage;
            }
            finally
            {
                _manager.mCloseConnection();
            }

            return View(model);
        }




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

                long userId;

                _manager.mOpenConnection();

                try
                {
                    string ip = HttpContext.Request.UserHostAddress;
                    userId = _manager.isAuthenticated(model.UserName.Trim(), model.Password.Trim(), ip);

                    if (userId != -1)
                    {
                        //string userDataString;
                        //userDataString = userId + ";" + model.UserName.Trim() + ";";

                        //HttpCookie authCookie = FormsAuthentication.GetAuthCookie(model.UserName, model.RememberMe);
                        ////Get the FormsAuthenticationTicket out of the encrypted cookie
                        //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                        ////Create a new FormsAuthenticationTicket that includes our custom User Data
                        //FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userDataString);

                        ////Update the authCookie's Value to use the encrypted version of newTicket
                        //authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                        ////Manually add the authCookie to the Cookies collection

                        //Response.Cookies.Add(authCookie);

                        //string temp;
                        //temp = FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe);

                        //Debug.WriteLine("FormsAuthentication.GetRedirectUrl " + temp);

                        //System.Web.HttpContext.Current.User = new MyUsers.MyCustomPrincipal(new MyUsers.MyCustomIndentity(userId, model.UserName));
                        // System.Web.HttpContext.Current.
                        //System.Threading.Thread.CurrentPrincipal = new MyUsers.MyCustomPrincipal(new MyUsers.MyCustomIndentity(userId, model.UserName));
                        //System.Web.HttpContext.Current.Session["MySessionData"] = userId;

                        if (MY_CUSTOM_IDENTITY == true)
                        {
                            string userDataString;
                            userDataString = userId + ";" + model.UserName.Trim() + ";";
                            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(model.UserName, model.RememberMe);
                            //Get the FormsAuthenticationTicket out of the encrypted cookie
                            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                            //Create a new FormsAuthenticationTicket that includes our custom User Data
                            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userDataString);

                            //Update the authCookie's Value to use the encrypted version of newTicket
                            authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                            //Manually add the authCookie to the Cookies collection
                            Response.Cookies.Add(authCookie);
                        }
                        else
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        }


                        MyManagerCSharp.Log.LogUserManager log = new MyManagerCSharp.Log.LogUserManager(_manager.mGetConnection());

                        if (TempData["AREA"] != null && TempData["AREA"].ToString() == "Mobile")
                        {
                            log.insert(userId, model.UserName.Trim(), MyManagerCSharp.Log.LogUserManager.LogType.LoginMobile, System.Net.IPAddress.Parse(ip));
                        }
                        else
                        {
                            log.insert(userId, model.UserName.Trim(), MyManagerCSharp.Log.LogUserManager.LogType.Login, System.Net.IPAddress.Parse(ip));
                        }


                        /** SESSIONE **/
                        MyManagerCSharp.MySessionData session = new MyManagerCSharp.MySessionData(userId);
                        session.Login = _manager.getLogin(userId);
                        //   session.Roles = manager.getRoles(userId);
                        // session.Profili = manager.getProfili(userId);
                        // session.Groups = manager.getGroupSmall(userId);

                        Session["MySessionData"] = session;

                        string temp;
                        temp = FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe);

                        Debug.WriteLine("FormsAuthentication.GetRedirectUrl " + temp);


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
                        sendMailExceptionAsync(ex);
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

                //MyUsers.SimpleSessionPersister.Username = model.UserName;
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }



        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            MyManagerCSharp.Log.LogUserManager log = new MyManagerCSharp.Log.LogUserManager("log");
            log.mOpenConnection();
            try
            {
                string ip = Request.UserHostAddress;
                log.insert(MySessionData.UserId, MySessionData.Login, MyManagerCSharp.Log.LogUserManager.LogType.Logout, ip);
            }
            catch (Exception ex)
            {
                //potrebbe esserci un errore in quanto in fase di sviluppo si memorizza un id di sessione diverso
                //il problema si presenta ad esemoio se si cambia il puntamento al DB

                //ignoro l'errore
                Debug.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                log.mCloseConnection();
            }


            MySessionData.LogOff();


            //if (TempData["AREA"] != null && TempData["AREA"].ToString() == "Mobile")
            //{
            //    return RedirectToAction("Index", "Mobile", new { area = "Mobile" });
            //}

            //if (TempData["AREA"] != null && TempData["AREA"].ToString() == "Admin")
            //{
            //    return RedirectToAction("Index", "Admin", new { area = "Admin" });
            //}


            return RedirectToAction("Index", "Home");
        }




        public ActionResult ResetPassword()
        {
            ResetPasswordModel model = new ResetPasswordModel();

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Login o email errati");
                return View(model);
            }


            //MyUsers.UserManager manager = new UserManager("utenti");
            long userId;
            string passwordGenerata = "";

            _manager.mOpenConnection();

            MyUsers.Models.MyUser u = null;

            try
            {
                userId = _manager.getUserIdFromLoginAndEmail(model.Login, model.Email);

                if (userId == -1)
                {
                    ModelState.AddModelError("", "La Login o l'e-mail inserite non sono corrette");
                    return View(model);
                }
                passwordGenerata = _manager.resetPassword(userId);

                u = _manager.getUser(userId);
            }
            catch (MyManagerCSharp.MyException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            finally
            {
                _manager.mCloseConnection();
            }


            if (u != null)
            {

                MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);

                mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Generazione Nuova Password";

                mail.Body = mail.getBodyResetPassword(u.nome, u.cognome, passwordGenerata, MyManagerCSharp.MailMessageManager.Lingua.IT);

                try
                {
                    mail.To(model.Email.Trim());
                    mail.send();
                }
                catch (Exception ex)
                {
                    MyManagerCSharp.MailManager.send(ex, "Reset password con email: " + model.Email + " - login: " + model.Login);
                }

            }


            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public ActionResult ChangePassword()
        {
            ChangePasswordModel model = new ChangePasswordModel();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                //ModelState.AddModelError("", "Login o email errati");
                return View(model);
            }


            //MyUsers.UserManager manager = new UserManager("utenti");
            _manager.mOpenConnection();


            try
            {
                long test;
                test = _manager.isAuthenticated((User.Identity as MyCustomIdentity).Login, model.OldPassword);


                if (test != (User.Identity as MyCustomIdentity).UserId)
                {
                    ModelState.AddModelError("", "Verificare la password inserita");
                    return View(model);
                }


                _manager.updatePassword((User.Identity as MyCustomIdentity).UserId, model.NewPassword);
            }
            catch (MyManagerCSharp.MyException ex)
            {
                if (ex.ErrorCode == MyManagerCSharp.MyException.ErrorNumber.LoginPasswordErrati)
                {
                    ModelState.AddModelError("", "Verificare la password inserita");
                }
                else
                {
                    ModelState.AddModelError("", ex.Message);
                }
                return View(model);
            }
            finally
            {
                _manager.mCloseConnection();
            }

            return RedirectToAction("Index", "Home");
        }






        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewData["TestoPrivacy"] = testoPrivacy;
            RegisterModel model = new RegisterModel();

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            Debug.WriteLine(String.Format("Tipo utenza: {0}", model.tipoUtenza));
            Debug.WriteLine(String.Format("Login: {0}", model.login));
            Debug.WriteLine(String.Format("Privacy: {0}", model.privacy));

            if (!ModelState.IsValid)
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        Debug.WriteLine(String.Format("ModelError: {0} ", error.ErrorMessage));
                    }
                }
            }

            if (model.tipoUtenza == "A" && String.IsNullOrEmpty(model.ragioneSociale))
            {
                ModelState.AddModelError("ragioneSociale", "Occorre specificare la ragione sociale dell'agenzia immobiliare");
            }

            if (model.tipoUtenza == "P" && String.IsNullOrEmpty(model.nome))
            {
                ModelState.AddModelError("nome", "Il nome è un valore obbligatorio");
            }

            if (model.tipoUtenza == "P" && String.IsNullOrEmpty(model.cognome))
            {
                ModelState.AddModelError("cognome", "Il cognome è un valore obbligatorio");
            }

            if (String.IsNullOrEmpty(model.privacy) || model.privacy == "NO")
            {
                ModelState.AddModelError("privacy", "Per proseguire con la registrazione occorre dare il consenso sulla privacy");
            }



            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    _manager.mOpenConnection();

                    long userId;
                    userId = _manager.getUserIdFromLogin(model.login);

                    if (userId != -1)
                    {
                        ModelState.AddModelError("", "Spiacenti ma occorre modificare la login in quanto è già stata utilizzata");
                        ViewData["TestoPrivacy"] = testoPrivacy;
                        return View(model);
                    }


                    long customerId = -1;
                    //Se si tratta di un'agenzia ....
                    if (model.tipoUtenza == "A")
                    {
                        MyUsers.CustomerManager customerManager = new MyUsers.CustomerManager(_manager.mGetConnection());

                        MyUsers.Models.MyCustomer c = new MyUsers.Models.MyCustomer();
                        c.ragioneSociale = model.ragioneSociale;

                        customerId = customerManager.insert(c);
                    }


                    long newUserID = -1;

                    MyUsers.Models.MyUser u = new MyUsers.Models.MyUser();
                    u.isEnabled = !bool.Parse(System.Configuration.ConfigurationManager.AppSettings["utenti.confirm.registration"]);

                    u.login = model.login;
                    u.email = model.email;

                    if (customerId != -1)
                    {
                        u.customerId = customerId;
                        u.nome = model.ragioneSociale;
                    }
                    else
                    {
                        u.nome = model.nome;
                        u.cognome = model.cognome;
                    }


                    newUserID = _manager.insert(u);


                    if (newUserID != -1)
                    {

                        //*************
                        //*** EMAIL ***
                        //*************
                        //invio un'email di conferma registrazione

                        MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);

                        mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Registrazione Portale";
                        mail.To(model.email);

                        //MY-DEBUGG
                        if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]))
                        {
                            mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);
                        }

                        string temp;

                        if (customerId != -1)
                        {
                            temp = model.ragioneSociale;
                        }
                        else
                        {
                            temp = model.nome;
                        }

                        mail.Body = mail.getBodyRegistrazioneUtente(temp, model.cognome, model.login, model.email, MyManagerCSharp.MailMessageManager.Lingua.IT);

                        //Me.telefono.Text, Me.indirizzo.Text, Me.numero_civico.Text, 
                        //Me.cap.Text, Me.cmbRegioneProvinciaComune1._getProvinciaText, Me.cmbRegioneProvinciaComune1._getComuneText)
                        mail.send();

                        //'15/09/2010 
                        //'L'email sulla password NON me la invio
                        //'Mi invio solo un messaggio di pro-memoria sulla nuova registrazione
                        mail.BccClearField();


                        if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["utenti.confirm.registration"]))
                        {
                            //*** se l'utente necessita della conferma per l'abilitazione dell'account 
                            //*** invio un'email all'amministratore per avvisarlo...
                            mail.To(System.Configuration.ConfigurationManager.AppSettings["mail.From"]);
                            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Attivazione nuovo account";
                            mail.Body = mail.getBodyAlertNewAccount(model.nome, model.cognome, model.login, newUserID, "", MyManagerCSharp.MailMessageManager.Lingua.IT);
                            mail.send();
                        }
                        else
                        {
                            //'**********************************
                            //'*** Generazione nuova password ***
                            string passwordGenerata;
                            passwordGenerata = _manager.resetPassword(newUserID);
                            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Generazione Nuova Password";
                            mail.Body = mail.getBodyResetPassword(model.nome, model.cognome, passwordGenerata, MyManagerCSharp.MailMessageManager.Lingua.IT);
                            mail.send();
                        }


                        // DIRECTORY per l'utente 
                        // NON la creo perchè altrimenti mi trovo una miriade di cartelle vuote 




                        //*** IMMOBILIARE ***
                        if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["Annunci.isEnabled"]))
                        {
                            Annunci.ImmobiliareManager managerImmobiliare = new Annunci.ImmobiliareManager("immobiliare");

                            try
                            {
                                //mi copio nelle tabelle del mercatino tutti questi dati per evitare di accedete al db degli utenti

                                managerImmobiliare.mOpenConnection();
                                //'27/02/2011
                                if (customerId != -1)
                                {
                                    managerImmobiliare.insertUser(newUserID, model.ragioneSociale, "", model.email, model.login, customerId);
                                }
                                else
                                {
                                    managerImmobiliare.insertUser(newUserID, model.nome, model.cognome, model.email, model.login, customerId);
                                }

                            }
                            catch (Exception ex)
                            {
                                //' caso di errore mi invio un'email di errore e continuo la registrazione normale
                                //'altrimenti dovrei cancellare il record che ho inseirto sul mdb
                                MyManagerCSharp.MailManager.send(ex);
                            }
                            finally
                            {
                                managerImmobiliare.mCloseConnection();
                            }
                        }


                        return RedirectToAction("Ok", "Home");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
                finally
                {
                    _manager.mCloseConnection();
                }
            }


            ViewData["TestoPrivacy"] = testoPrivacy;
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Disassociate(string provider, string providerUserId)
        //{
        //    string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
        //    ManageMessageId? message = null;

        //    // Only disassociate the account if the currently logged in user is the owner
        //    if (ownerAccount == User.Identity.Name)
        //    {
        //        // Use a transaction to prevent the user from deleting their last login credential
        //        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        //        {
        //            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //            if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
        //            {
        //                OAuthWebSecurity.DeleteAccount(provider, providerUserId);
        //                scope.Complete();
        //                message = ManageMessageId.RemoveLoginSuccess;
        //            }
        //        }
        //    }

        //    return RedirectToAction("Manage", new { Message = message });
        //}

        //
        // GET: /Account/Manage

        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : "";
        //    ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        //
        // POST: /Account/Manage

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Manage(LocalPasswordModel model)
        //{
        //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasLocalAccount)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
        //            bool changePasswordSucceeded;
        //            try
        //            {
        //                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
        //            }
        //            catch (Exception)
        //            {
        //                changePasswordSucceeded = false;
        //            }

        //            if (changePasswordSucceeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a local password so remove any validation errors caused by a missing
        //        // OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            catch (Exception)
        //            {
        //                ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // POST: /Account/ExternalLogin

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback

        //[AllowAnonymous]
        //public ActionResult ExternalLoginCallback(string returnUrl)
        //{
        //    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //    if (!result.IsSuccessful)
        //    {
        //        return RedirectToAction("ExternalLoginFailure");
        //    }

        //    if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // If the current user is logged in add the new account
        //        OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // User is new, ask for their desired membership name
        //        string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
        //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
        //        ViewBag.ReturnUrl = returnUrl;
        //        return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        //{
        //    string provider = null;
        //    string providerUserId = null;

        //    if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Insert a new user into the database
        //        using (UsersContext db = new UsersContext())
        //        {
        //            UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
        //            // Check if user already exists
        //            if (user == null)
        //            {
        //                // Insert name into the profile table
        //                db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
        //                db.SaveChanges();

        //                OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
        //                OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

        //                return RedirectToLocal(returnUrl);
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
        //            }
        //        }
        //    }

        //    ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //[AllowAnonymous]
        //[ChildActionOnly]
        //public ActionResult ExternalLoginsList(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveExternalLogins()
        //{
        //    ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
        //    List<ExternalLogin> externalLogins = new List<ExternalLogin>();
        //    foreach (OAuthAccount account in accounts)
        //    {
        //        AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

        //        externalLogins.Add(new ExternalLogin
        //        {
        //            Provider = account.Provider,
        //            ProviderDisplayName = clientData.DisplayName,
        //            ProviderUserId = account.ProviderUserId,
        //        });
        //    }

        //    ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        //}

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        //internal class ExternalLoginResult : ActionResult
        //{
        //    public ExternalLoginResult(string provider, string returnUrl)
        //    {
        //        Provider = provider;
        //        ReturnUrl = returnUrl;
        //    }

        //    public string Provider { get; private set; }
        //    public string ReturnUrl { get; private set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
        //    }
        //}

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
