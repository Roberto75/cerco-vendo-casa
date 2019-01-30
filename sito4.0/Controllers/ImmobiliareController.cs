using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Diagnostics;

namespace MyWebApplication.Controllers
{
    public class ImmobiliareController : MyBaseController
    {
       
        public const int MaxWidthImage = 500;
        public const int MaxHeightImage = 500;

        public const int WidthThumbnail = 200;
        public const int HeightThumbnail = 200;

        private const string SESSSION_FILTER_SEARCH = "SearchImmobiliareModel";

        private Annunci.ImmobiliareManager manager = new Annunci.ImmobiliareManager("immobiliare");

        private MyManagerCSharp.RegioniProvinceComuniManager regioniProvinceComuniManager = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");



        [AllowAnonymous]
        public ActionResult Index(Annunci.Models.SearchImmobili model)
        {
           // readRequestParameters(ref model);



            if (Request.HttpMethod.ToString() == "GET" && Session[SESSSION_FILTER_SEARCH] != null && Request.UrlReferrer != null
               && Request.UrlReferrer.LocalPath.IndexOf("/Libri/TestiScolastici") == -1
               //&& Request.UrlReferrer.LocalPath.IndexOf("/Libri/Details") == -1
               && Request.UrlReferrer.LocalPath != "/Libri/Categorie")
            {
                model = (Session[SESSSION_FILTER_SEARCH] as Annunci.Models.SearchImmobili );
                //model.filter = (Session[SESSSION_FILTER_SEARCH] as Annunci.Libri.Models.Libro);
                Debug.WriteLine("Leggo i parametri di ricerca dalla sessione ...");
                Debug.WriteLine("Filtro Days: " + model.days);
            }


            if (model.filter == null)
            {
                model.filter = new Annunci.Models.Immobile();
            }


            Debug.WriteLine("Regione: " + model.filter.regioneId);
            Debug.WriteLine("Provincia: " + model.filter.provinciaId);
            Debug.WriteLine("Comune: " + model.filter.comuneId);

            try
            {
                manager.mOpenConnection();

                model.comboRegioni = regioniProvinceComuniManager.getComboRegioni();
                if (model.filter.regioneId != null)
                {
                    model.comboProvince = regioniProvinceComuniManager.getComboProvince((int)model.filter.regioneId);
                }

                if (model.filter.provinciaId != null)
                {
                    model.comboComuni = regioniProvinceComuniManager.getComboComuni(model.filter.provinciaId);
                }

                manager.getList(model);
            }
            finally
            {
                manager.mCloseConnection();
            }


            //Annunci.Models.PagedModelmmobili model = new Annunci.Models.PagedModelmmobili
            //{
            //    PageSize = pageSize,
            //    PageNumber = page,
            //    Immobili = risultato,
            //    TotalRows = totalRecords
            //};



            //la prima volta!
            if (model.Tempo == 0)
            {
                //model = new Annunci.Models.SearchParameter();
                model.Tempo = Annunci.Models.SearchImmobili.EnumTempo.Tutti;
            }

            // model.PageSize = pageSize;
            //model.PageNumber = page;
            //model.Immobili = risultato;
            // model.TotalRows = totalRecords;
            //model.Sort = sort;
            //model.SortDir = sortDir;

            //model.RegioneProvinciaComune = new Models.ModelRegioneProvinciaComune();


            //if (String.IsNullOrEmpty (Categoria)){
            //    model.categoria = 0;
            //}else{
            //    model.categoria = int.Parse(Categoria);
            //}

            Session[SESSSION_FILTER_SEARCH] = model;
            //Session["SearchModel"] = model;
            return View(model);
        }

        //[AcceptVerbs (HttpVerbs.Post )]
        //public ActionResult Search(Models.ModelImmobiliareSearch model, int page = 1, string sort = "ANNUNCIO.date_added", string sortDir = "DESC")
        //{
        //    const int pageSize = 5;
        //    int totalRecords;


        //    List<Annunci.Models.Immobile> risultato;
        //    Annunci.ImmobiliareManager manager = new ImmobiliareManager("immobiliare");
        //    manager.mOpenConnection();
        //    try
        //    {
        //        risultato = manager.getList(out totalRecords, pageSize, page, sort, sortDir);
        //    }
        //    finally
        //    {
        //        manager.mCloseConnection();
        //    }


        //    //Annunci.Models.PagedModelmmobili model = new Annunci.Models.PagedModelmmobili
        //    //{
        //    //    PageSize = pageSize,
        //    //    PageNumber = page,
        //    //    Immobili = risultato,
        //    //    TotalRows = totalRecords
        //    //};

        //    //model.RegioneProvinciaComune = new Models.ModelRegioneProvinciaComune();
        //    //model.RegioneProvinciaComune.Regione = 

        //    model.PageSize = 5;
        //    model.TotalRows  = totalRecords;
        //    model.Immobili = risultato;



        //    return View(model);
        //}


        [AllowAnonymous]
        public ActionResult Details(long id = 0)
        {
            Annunci.Models.Immobile immobile = null;
            manager.mOpenConnection();
            try
            {
                immobile = manager.getImmobile(id);

                if (immobile == null)
                {
                    return HttpNotFound();
                }

                //CLICK
                if (User.Identity.IsAuthenticated)
                {
                    //if ((User.Identity as MyUsers.MyCustomIdentity).UserId != immobile.userId)
                    if (MySessionData != null && MySessionData.UserId != immobile.userId)
                    {
                        //se non si tratta di un mio annuncio ...
                        manager.annuncioAddClick(id);
                    }
                }
                else
                {
                    //nel caso di connessioni anonime non posso fare nulla
                    manager.annuncioAddClick(id);
                }


                // PHOTO
                Annunci.PhotoManager photoManager = new Annunci.PhotoManager(manager.mGetConnection());
                List<Annunci.Models.MyPhoto> photos;
                photos = photoManager.getMyPhotosIsNotPlanimetria(id);
                Debug.WriteLine("Trovate {0} immagini", photos.Count);
                ViewBag.photos = photos;



                //KEYWORDS
                string temp;
                temp = immobile.categoria.ToString();

                if (immobile.immobile != null)
                {
                    temp += " " + immobile.immobile.ToString();
                }

                if (!String.IsNullOrEmpty(immobile.comune))
                {
                    temp += " " + immobile.comune;
                }


                if (!String.IsNullOrEmpty(immobile.cap))
                {
                    temp += " " + immobile.cap;
                }

                if (!String.IsNullOrEmpty(immobile.indirizzo))
                {
                    temp += " " + immobile.indirizzo;
                }


                ViewBag.Description = temp;
                ViewBag.Keywords = "cerco casa, vendo casa, annunci immobiliari, " + immobile.categoria.ToString() + ", " + immobile.immobile.ToString();

                ViewBag.Title = "Cerco Vendo Casa: " + temp;

                if (immobile.customerId == -1)
                {
                    ViewBag.Title += " , Solo privati , no agenzie";
                }

            }
            finally
            {
                manager.mCloseConnection();
            }

            Debug.WriteLine("Climatizzato: " + immobile.climatizzato);
            Debug.WriteLine("speseCondominiali: " + immobile.speseCondominiali);

            return View(immobile);
        }

        [AllowAnonymous]
        public ActionResult GetMap(Annunci.Models.SearchImmobili model)
        {

            readRequestParameters(ref model);



            manager.mOpenConnection();

            List<Annunci.Models.Immobile> risultato = new List<Annunci.Models.Immobile>();

            try
            {
                //risultato = _manager.getList(model, out totalRecords, 0, 0, "ANNUNCIO.date_added", "");
            }
            finally
            {
                manager.mCloseConnection();
            }


            Session["SearchModel"] = model;

            //  return Json(risultato.Select(x => new { latitude = x.latitude, longitude = x.longitude }), JsonRequestBehavior.AllowGet);

            return Json(risultato.Where(y => y.longitude > 0 && y.latitude > 0).Select(x => new { latitude = x.latitude, longitude = x.longitude, titolo = MyHelper.getTitoloImmobile(x).ToHtmlString(), id = x.immobileId }), JsonRequestBehavior.AllowGet);
        }



        private void readRequestParameters(ref Annunci.Models.SearchImmobili model)
        {

            Debug.WriteLine("*** FILTRI DI RICERCA ***");
            Debug.WriteLine("SORT: " + model.Sort + " DIR: " + model.SortDir);

            if (model.Sort == "date_added" || model.Sort == "date")
            {
                model.Sort = "ANNUNCIO.date_added";
            }


            if (Request.HttpMethod.ToString() == "GET" && Session["SearchModel"] != null)
            {
                model = (Session["SearchModel"] as Annunci.Models.SearchImmobili);
                Debug.WriteLine("Leggo i parametri di ricerca dalla sessione ...");
            }
            else
            {
                //model.PageSize = (Request.Form["pageSize"] == null) ? 10 : int.Parse(Request.Form["pageSize"]);
                //model.PageNumber = (Request.Form["page"] == null) ? 1 : int.Parse(Request.Form["page"]);
                //model.Sort = (Request.Form["sort"] == null) ? "ANNUNCIO.date_added" : Request.Form["sort"]; ;
                //model.SortDir = (Request.Form["sortDir"] == null) ? "DESC" : Request.Form["sortDir"]; ;



                Debug.WriteLine(String.Format("RegioneId: {0} \t ProvinciaId: {1} \t ComuneId: {2}", Request.Form["regioneId"], Request.Form["provinciaId"], Request.Form["comuneId"]));
                Debug.WriteLine(String.Format("Tipo annuncio: {0}", Request.Form["TipoAnnnuncio"]));
                //   Debug.WriteLine(String.Format("ascensore: {0}", Request.Form["filter.ascensore"]));

                Debug.WriteLine(String.Format("Tempo: {0}", model.Tempo.ToString()));


                TryUpdateModel(model.filter, "filter");

                Debug.WriteLine(String.Format("Immobile: {0}", model.filter.immobile));
                Debug.WriteLine(String.Format("Categoria: {0}", model.filter.categoria));
                Debug.WriteLine(String.Format("Tipo Vista: {0}", Request.Form["tipoVista"]));

                Debug.WriteLine(String.Format("Prezzo MAX: {0}", model.prezzoMax));
                Debug.WriteLine(String.Format("Mq MIN: {0}", model.mqMin));

                Debug.WriteLine(String.Format("Posto auto: {0}", model.filter.postoAuto.ToString()));
                Debug.WriteLine(String.Format("Ascensore: {0}", model.filter.ascensore.ToString()));


                if (Request.Form["tipoVista"] == null || Request.Form["tipoVista"] == "")
                {
                    model.TipoVista = Annunci.Models.SearchImmobili.EnumTipoVista.Elenco;
                }
                else
                {
                    model.TipoVista = (Annunci.Models.SearchImmobili.EnumTipoVista)Enum.Parse(typeof(Annunci.Models.SearchImmobili.EnumTipoVista), Request.Form["tipoVista"].ToString());
                }



                if (!String.IsNullOrEmpty(Request.Form["regioneId"]))
                {
                    model.filter.regioneId = int.Parse(Request.Form["regioneId"]);
                }

                model.filter.provinciaId = Request.Form["provinciaId"];
                model.filter.comuneId = Request.Form["comuneId"];

                if (Request.Form["TipoAnnnuncio"] != null)
                {
                    model.TipoAnnuncio.Clear();

                    if (Request.Form["TipoAnnnuncio"].Contains("Privato"))
                    {
                        model.TipoAnnuncio.Add(Annunci.Models.SearchImmobili.EnumTipoAnnuncio.Privato);
                    }

                    if (Request.Form["TipoAnnnuncio"].Contains("Agenzia"))
                    {
                        model.TipoAnnuncio.Add(Annunci.Models.SearchImmobili.EnumTipoAnnuncio.Agenzia);
                    }

                }
            }
        }



        [Authorize]
        public ActionResult Create(Models.CreateModel model)
        {
            model.comboRegioni = regioniProvinceComuniManager.getComboRegioni();

            if (model.immobile.regioneId != null)
            {
                model.comboProvince = regioniProvinceComuniManager.getComboProvince((int)model.immobile.regioneId);
            }

            if (model.immobile.provinciaId != null)
            {
                model.comboComuni = regioniProvinceComuniManager.getComboComuni(model.immobile.provinciaId);
            }

            //model.immobile = new Annunci.Models.Immobile();
            //model.immobile.giardinoMq = 0;

            Debug.WriteLine(String.Format("Categoria: {0}", model.immobile.categoria));
            Debug.WriteLine(String.Format("Tipo annuncio: {0}", model.immobile.immobile));
            return View(model);
        }


        [Authorize]
        [ActionName("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(Models.CreateModel model)
        {


            //if (String.IsNullOrEmpty(Request["MyAction"]))
            //{
            //    return RedirectToAction("Error", "Home");
            //}

            //if (Request["MyAction"] != "Indietro" && Request["MyAction"] != "Avanti")
            //{
            //    return RedirectToAction("Error", "Home");
            //}

            //if (Request["MyAction"] == "Indietro")
            //{
            //    return RedirectToAction("Index", "Immobiliare");
            //}




            if (model.immobile.categoria == null)
            {
                ModelState.AddModelError("", "La categoria è un campo obbligatorio");
            }

            if (model.immobile.immobile == null)
            {
                ModelState.AddModelError("", "Il tipo di immobile è un campo obbligatorio");
            }

            if (model.immobile.regioneId == null || model.immobile.regioneId == -1)
            {
                ModelState.AddModelError("Specificare la località dell'immobile", "Specificare la località dell'immobile. La regione è un campo obbligatorio");
            }

            if (String.IsNullOrEmpty(model.immobile.provinciaId) || model.immobile.provinciaId == "-1" || model.immobile.provinciaId == "---")
            {
                ModelState.AddModelError("Specificare la località dell'immobile", "Specificare la località dell'immobile. La provincia è un campo obbligatorio");
            }

            if (String.IsNullOrEmpty(model.immobile.comuneId) || model.immobile.comuneId == "-1" || model.immobile.comuneId == "---")
            {
                ModelState.AddModelError("Specificare la località dell'immobile", "Specificare la località dell'immobile. Il comune è un campo obbligatorio");
            }

            if (!ModelState.IsValid)
            {
                model.comboRegioni = regioniProvinceComuniManager.getComboRegioni();

                return View(model);
            }


            //if (String.IsNullOrEmpty(model.regione) || String.IsNullOrEmpty(model.provincia) || String.IsNullOrEmpty(model.comune))
            //{
            //    MyManagerCSharp.RegioniProvinceComuniManager m = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");
            //    try
            //    {
            //        m.mOpenConnection();

            //        model.regione = m.getRegioneById(model.regioneId);
            //        model.provincia = (m.getProvinciaBySigla(model.provinciaId)).Rows[0]["VALORE"].ToString();
            //        model.comune = (m.getComuneByCodiceISTAT(model.comuneId)).Rows[0]["VALORE"].ToString();
            //    }
            //    finally
            //    {
            //        m.mCloseConnection();
            //    }
            //}

            model.comboComuni = null;
            model.comboProvince = null;

            Debug.WriteLine("MQ: " + model.immobile.MQ);
            Debug.WriteLine("Stato: " + model.immobile.statoImmobile);
            Debug.WriteLine("Camere da letto: " + model.immobile.camereDaLetto);
            //return RedirectToAction("CreateStep2", "Immobiliare", model);

            return View("CreateStep2", model);
        }
        /*

        [Authorize]
        [HttpGet]
        public ActionResult CreateStep2(Models.CreateModel model)
        {
            //model.comboProvince = new List<MyManagerCSharp.Models.MyItem>();
            //model.comboComuni = new List<MyManagerCSharp.Models.MyItem>();

            if (!ModelState.IsValid)
            {

                // var error = ModelState.SelectMany(x => x.Value.Errors);
                foreach (var value in ModelState.Values)
                {
                    foreach (var merror in value.Errors)
                    {

                        //throw new Exception(merror.ErrorMessage, merror.Exception);
                        Debug.WriteLine(merror.ErrorMessage + merror.Exception);

                    }

                }

                return RedirectToAction("Create", "Immobiliare", model);
            }

            return View(model);
        }
        */
        /*
        [Authorize]
        [HttpPost]
        [ActionName("CreateStep2")]
        [ValidateInput(false)]
        public ActionResult CreateStep2Post(Models.CreateModel model)
        {
            Debug.WriteLine("MyAction: " + Request["MyAction"]);

            // Controllo sul parametro MyAction
            if (String.IsNullOrEmpty(Request["MyAction"]))
            {
                return RedirectToAction("Error", "Home");
            }

            if (Request["MyAction"] != "Indietro" && Request["MyAction"] != "Avanti")
            {
                return RedirectToAction("Error", "Home");
            }

            if (Request["MyAction"] == "Indietro")
            {

                return View("Create", model);
                //return RedirectToAction("Create", "Immobiliare", model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("Preview", "Immobiliare", model); ;
        }*/

        [Authorize]
        [HttpPost]
        public ActionResult Preview(Models.CreateModel model)
        {

            Debug.WriteLine("longitude: " + model.immobile.longitude);
            Debug.WriteLine("latitude: " + model.immobile.latitude);

            if (!ModelState.IsValid)
            {

                var error = ModelState.SelectMany(x => x.Value.Errors);
                foreach (var value in ModelState.Values)
                {
                    foreach (var merror in value.Errors)
                    {
                        //throw new Exception(merror.ErrorMessage, merror.Exception);
                        Debug.WriteLine(merror.ErrorMessage + merror.Exception);
                    }
                }

                return RedirectToAction("CreateStep2", "Immobiliare", model);
            }

            // PREVIEW
            model.immobile.dataInserimento = DateTime.Now;
            //   model.login = (User.Identity as MyUsers.MyCustomIdentity).Login;
            model.immobile.login = MySessionData.Login;

            //Debug.WriteLine("Nota:" + Request["nota"]);
            //Debug.WriteLine("Nota:" + model.nota);


            return View("Preview", model.immobile);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Insert(Annunci.Models.Immobile model)
        {

            Debug.WriteLine("Climatizzato: " + model.climatizzato);
            Debug.WriteLine("speseCondominiali: " + model.speseCondominiali);


            //Controllo sul Modello
            if (!ModelState.IsValid)
            {
                return RedirectToAction("CreateStep2", "Immobiliare", model);
            }


            manager.mOpenConnection();

            //Debug.WriteLine("Nota:" + Request["nota"]);
            //Debug.WriteLine("Nota:" + model.nota);

            Debug.WriteLine("longitude: " + model.longitude);
            Debug.WriteLine("latitude: " + model.latitude);

            try
            {
                model.nota = HttpUtility.HtmlDecode(model.nota);
                //  _manager.insertAnnuncio(model, (User.Identity as MyUsers.MyCustomIdentity).UserId);
                manager.insertAnnuncio(model, MySessionData.UserId);
            }
            finally
            {
                manager.mCloseConnection();
            }



            return RedirectToAction("Ok", "Home");
        }


        #region "Trattativa"

        [Authorize]
        public ActionResult Reply(long annuncioId, long? trattativaId, long? quote, long? rispostaId)
        {
            Models.ReplyModel model;
            model = getReplyModel(annuncioId, trattativaId, quote, rispostaId);

            return View(model);
        }


        private Models.ReplyModel getReplyModel(long annuncioId, long? trattativaId, long? quote, long? rispostaId)
        {

            Models.ReplyModel model = new Models.ReplyModel();
            model.annuncioId = annuncioId;

            if (trattativaId == null)
            {
                model.trattativaId = -1;
            }
            else
            {
                model.trattativaId = (long)trattativaId;
            }


            try
            {
                manager.mOpenConnection();

                Annunci.Models.Immobile immobile;
                immobile = manager.getImmobile(annuncioId);

                model.annuncio = immobile;

                if (quote != null)
                {
                    Annunci.Models.Risposta risposta;
                    risposta = manager.getRisposta((long)quote);
                    model.testo = "<blockquote><hr/><b>" + risposta.login + "</b> scrive: </br></br>" + risposta.testo + "<hr/></blockquote>";
                    model.rispostaId = (long)quote;
                    model.replyTo = risposta.login;
                }
                else
                {
                    model.testo = "";
                    if (rispostaId == null)
                    {
                        model.rispostaId = -1;
                    }
                    else
                    {
                        model.rispostaId = (long)rispostaId;
                        Annunci.Models.Risposta risposta;
                        risposta = manager.getRisposta((long)rispostaId);
                        model.replyTo = risposta.login;
                    }

                }

            }
            finally
            {
                manager.mCloseConnection();
            }

            return model;
        }



        [Authorize]
        [HttpPost, ActionName("Reply")]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyPost(long annuncioId, string testo, long? rispostaId, long? trattativaId)
        {
            //long userId = (User.Identity as MyUsers.MyCustomIdentity).UserId;
            long userId = MySessionData.UserId;


            if (String.IsNullOrEmpty(testo.Trim()))
            {
                ModelState.AddModelError("", "Inserire un messaggio");

                Models.ReplyModel model;
                model = getReplyModel(annuncioId, trattativaId, null, rispostaId);

                model.testo = testo;

                return View(model);
            }

            bool isOwner = true;
            if (ModelState.IsValid)
            {
                try
                {
                    manager.mOpenConnection();


                    if (rispostaId == null || rispostaId == -1)
                    {
                        //' si tratta di una nuova trattativa!!
                        trattativaId = manager.insertTrattativa(annuncioId, userId);
                        manager.rispondi((long)trattativaId, userId, testo);
                    }
                    else
                    {
                        manager.rispondi((long)trattativaId, userId, testo, (long)rispostaId);
                    }


                    isOwner = manager.isOwner(annuncioId, MySessionData.UserId);

                    Annunci.Models.Immobile immobile;
                    immobile = manager.getImmobile(annuncioId);

                    //*** EMAIL ***
                    System.Data.DataTable dt;
                    dt = manager.getEmailReplyAnnnuncio((long)trattativaId);

                    Annunci.Libri.LibriMailMessageManager mail = new Annunci.Libri.LibriMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                    mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Nuovo messaggio";
                    mail.Body = mail.getBodyNuovoMessaggioReply((long)trattativaId, annuncioId, immobile.immobile.ToString(), String.Format("Immobiliare/Trattativa/{0}", trattativaId));

                    if (isOwner)
                    {
                        mail.To(dt.Rows[0]["email"].ToString());
                    }
                    else
                    {
                        mail.To(dt.Rows[0]["email_owner"].ToString());
                    }

                    //MY-DEBUGG
                    //' mail._ToClearField()
                    //'mail._To("roberto.rutigliano@gmail.com")

                    mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);
                    mail.send();


                    //'l'inserimento di una nuova risposta comporta la notifica del messaggio
                    //'ci passo l'id dell'utente per facilitare la ricerca 
                    if (isOwner)
                    {
                        manager.notificaUser((long)trattativaId, long.Parse(dt.Rows[0]["user_id"].ToString()));
                    }
                    else
                    {
                        manager.notificaOwner((long)trattativaId, long.Parse(dt.Rows[0]["user_id_owner"].ToString()));
                    }
                }
                finally
                {
                    manager.mCloseConnection();
                }
            }

            //return RedirectToAction("Details", new { id = annuncioId });
            if (isOwner)
            {
                return RedirectToAction("MyAnnunci");
            }
            return RedirectToAction("MyTrattative");
        }




        [Authorize]
        public ActionResult MyTrattative()
        {


            manager.mOpenConnection();

            List<Annunci.Models.Trattativa> risultato;

            try
            {
                risultato = manager.getListTrattative(MySessionData.UserId, Annunci.Models.Trattativa.TipoTrattativa.Immobile);
            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(risultato);
        }





        [Authorize]
        public ActionResult Trattativa(long? id)
        {
            Debug.WriteLine("trattativaId: " + id);
            Models.ModelTrattativa model = new Models.ModelTrattativa();
            
            manager.mOpenConnection();
                       

            try
            {
                model.trattativa = manager.getTrattativa((long)id);
                if (model.trattativa == null)
                {
                    return View("~/Views/Errors/NotAvailable.cshtml");
                }

                if (!manager.authorizeShowTrattativa(MySessionData.UserId, (long)id))
                {
                    return RedirectToAction("NotAuthorized", "Errors");
                }


                manager.setRisposteFromTrattativa(model.trattativa);

                model.immobile = manager.getImmobile(model.trattativa.annuncioId);

                if (model.immobile == null)
                {
                    //vuol dire che l'annuncio è stato rimosso ... 
                    //return View("NotAvailable");

                    // il controllo lo faccio sulla VIEW così visualizzo i pulsanti corretti
                }

                //Verifico se l'utente che ha inserito la risposta sia il prorietario dell'annuncio
                bool isOwner;
                isOwner = manager.isOwner(model.trattativa.annuncioId, MySessionData.UserId);

                if (isOwner)
                {
                    manager.updateNotificaLetturaRispostaOwner((long)id);
                }
                else
                {
                    manager.updateNotificaLetturaRispostaUser((long)id);
                }

            }
            finally
            {
                manager.mCloseConnection();
            }

            if (TempData["AREA"] != null && TempData["AREA"].ToString() == "Admin")
            {
                return View("~/Areas/Admin/Views/Annunci/Trattativa.cshtml", model);
            }

            return View(model);
        }


        


        #endregion




        public ActionResult MyAnnunci()
        {
            manager.mOpenConnection();

            List<Annunci.Models.Immobile> risultato;

            System.Collections.Hashtable hashtableRisposte = new System.Collections.Hashtable();

            try
            {

                //if (User.Identity is System.Web.Security.FormsIdentity)
                //{
                //    risultato = manager.getListAnnunci((User.Identity as System.Web.Security.FormsIdentity)..UserId);
                //}

                //if (User.Identity is MyUsers.MyCustomIdentity)
                //{
                //    risultato = manager.getListAnnunci((User.Identity as MyUsers.MyCustomIdentity).UserId);
                //}


                risultato = manager.getListAnnunci(MySessionData.UserId);

                Annunci.AnnunciManager annuncioManager = new Annunci.AnnunciManager(manager.mGetConnection());
                long numeroRisposte;
                foreach (Annunci.Models.Immobile i in risultato)
                {

                    numeroRisposte = annuncioManager.getNumeroRisposteOfAnnuncio(i.immobileId, MySessionData.UserId);
                    hashtableRisposte.Add(i.immobileId, numeroRisposte);
                }
            }
            finally
            {
                manager.mCloseConnection();
            }

            ViewData["hashtableRisposte"] = hashtableRisposte;
            return View(risultato);
        }

        [Authorize]
        public ActionResult MyAnnuncio(long id)
        {
            Debug.WriteLine("MyAnnuncioId: " + id);

            Models.UpdateModel model;
            model = new Models.UpdateModel();

            Annunci.Models.Immobile immobile = null;
            manager.mOpenConnection();
            try
            {
                immobile = manager.getImmobile(id);

                if (immobile == null)
                {
                    return HttpNotFound();
                }

                if (immobile.userId != MySessionData.UserId)
                {
                    return RedirectToAction("NotAuthorized", "Home");
                }

                // PHOTO
                Annunci.PhotoManager photoManager = new Annunci.PhotoManager(manager.mGetConnection());

                List<Annunci.Models.MyPhoto> photos;
                photos = photoManager.getMyPhotosIsNotPlanimetria(id);

                Debug.WriteLine("Trovate {0} immagini", photos.Count);

                if (Request.IsAjaxRequest())
                {
                    Models.GalleryModel modelGallery = new Models.GalleryModel();
                    modelGallery.externalId = immobile.immobileId;
                    modelGallery.photos = photos;
                    return PartialView("GalleryEdit", modelGallery);
                }

                model.photos = photos;
            }
            finally
            {
                manager.mCloseConnection();
            }

            model.immobile = immobile;
            return View(model);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long? annuncioId)
        {

            if (annuncioId == null)
            {
                return HttpNotFound();
            }
            Debug.WriteLine("Delete MyAnnuncio: " + annuncioId);


            Annunci.Models.Immobile immobile;
            try
            {
                manager.mOpenConnection();

                immobile = manager.getImmobile((long)annuncioId);
                if (immobile == null)
                {
                    return HttpNotFound();
                }


            }
            finally
            {
                manager.mCloseConnection();
            }


            Debug.WriteLine("Delete MyAnnuncio: " + annuncioId);
            ViewData["Tipo"] = "Annuncio";
            ViewData["MyId"] = annuncioId;


            ViewBag.Title = "Cerco Vendo Casa: Cancellazione annuncio";

            return View(immobile);
        }




        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteExecute(long? MyId)
        {

            if (MyId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "DeleteExecute");
            }

            Debug.WriteLine("MyAnnuncioId: " + MyId);


            Annunci.AnnunciManager managerAnnuncio = new Annunci.AnnunciManager("immobiliare");
            try
            {
                managerAnnuncio.mOpenConnection();

                managerAnnuncio.deleteAnnuncioLogic((long)MyId, Annunci.AnnunciManager.StatoAnnuncio.Da_cancellare, Server.MapPath("~"));

            }
            finally
            {
                managerAnnuncio.mCloseConnection();
            }



            return RedirectToAction("MyAnnunci");
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateNota(long? annuncioId, string nota)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "UpdateNota");
            }

            nota = HttpUtility.HtmlDecode(nota);
            Debug.WriteLine("UpdateNota: " + annuncioId + " Nota: " + nota);

            try
            {

                manager.mOpenConnection();

                if (manager.updateAnnuncioDescrizione((long)annuncioId, nota, false) > 0)
                {

                    System.Data.DataTable dt;
                    dt = manager.getEmailUtentiInTrattativa((long)annuncioId);


                    if (dt.Rows.Count > 0)
                    {

                        Annunci.Models.Immobile i;
                        i = manager.getImmobile((long)annuncioId);

                        Annunci.ImmobiliareMailMessageManager mail = new Annunci.ImmobiliareMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                        mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Modifica annuncio";
                        mail.Body = mail.getBodyModificaTestoAnnuncio((long)annuncioId, i.categoria.ToString() + " - " + i.immobile.ToString(), String.Format("Immobiliare/Details/{0}", annuncioId));

                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            mail.Bcc(row["email"].ToString());
                        }
                        //'MY-DEBUGG
                        mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);

                        mail.send();
                    }
                }
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("MyAnnuncio", new { id = annuncioId });
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdatePrezzo(long? annuncioId, string nuovoPrezzo)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "annuncioId");
            }

            //if (nuovoPrezzo == null)
            if (String.IsNullOrEmpty(nuovoPrezzo))
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "prezzo");
            }

            decimal dNuovoPrezzo;
            dNuovoPrezzo = decimal.Parse(nuovoPrezzo.Replace(".", ","));

            Debug.WriteLine("UpdatePrezzo (String): " + nuovoPrezzo);
            Debug.WriteLine("UpdatePrezzo: " + dNuovoPrezzo);


            try
            {

                manager.mOpenConnection();

                if (manager.updateAnnuncioPrezzo((long)annuncioId, (decimal)dNuovoPrezzo, false) > 0)
                {

                    System.Data.DataTable dt;
                    dt = manager.getEmailUtentiInTrattativa((long)annuncioId);


                    if (dt.Rows.Count > 0)
                    {

                        Annunci.Models.Immobile i;
                        i = manager.getImmobile((long)annuncioId);

                        Annunci.ImmobiliareMailMessageManager mail = new Annunci.ImmobiliareMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                        mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Modifica annuncio";
                        mail.Body = mail.getBodyAggiornamentoPrezzoAnnuncio((long)annuncioId, i.categoria.ToString() + " - " + i.immobile.ToString(), i.prezzo, (decimal)dNuovoPrezzo, String.Format("Immobiliare/Details/{0}", annuncioId));

                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            mail.Bcc(row["email"].ToString());
                        }
                        //'MY-DEBUGG
                        mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);

                        mail.send();
                    }
                }
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("MyAnnuncio", new { id = annuncioId });
        }


        [Authorize]
        [HttpPost]
        public JsonResult AddImage(long? annuncioId, HttpPostedFileBase MyFile)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "DeleteExecute");
            }

            Debug.WriteLine("AddImage: " + annuncioId + " " + Request["MyFile"]);

            Models.JsonMessageModel model = new Models.JsonMessageModel();

            if (MyFile == null)
            {
                model.esito = "Failed";
                model.messaggio = "Selezionare un file";
                return Json(model);
            }



            //// Verify that the user selected a file
            //if (MyFile != null && MyFile.ContentLength > 0)
            //{
            //    //Verifico l'estenzione
            //    string  temp ;
            //    temp = MyFile.FileName;

            //    if (! temp.EndsWith(".gif") &&  !temp.EndsWith(".jpg") && ! temp.EndsWith(".jpeg") && ! temp.EndsWith(".png")) {
            //        ModelState.AddModelError("", "Sono ammessi solo file con estenzione .gif, .jpg oppure .png");
            //        return RedirectToAction("MyAnnuncio", new { id = annuncioId });
            //    }
            //}


            string folder;
            folder = System.Configuration.ConfigurationManager.AppSettings["mercatino.images.folder"];
            if (!System.IO.Directory.Exists(Server.MapPath(folder)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(folder));
            }
            folder = folder + annuncioId + "/";
            if (!System.IO.Directory.Exists(Server.MapPath(folder)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(folder));
            }


            System.Drawing.Image myImage = null;

            try
            {

                myImage = System.Drawing.Image.FromStream(MyFile.InputStream);
            }
            catch (Exception e)
            {
                //NON si tratta di un'immagine!
                model.esito = "Failed";
                model.messaggio = "Il file selezionato NON contiene un'immagine";
                return Json(model);
            }


            if (myImage.Height > MaxHeightImage)
            {
                myImage = Annunci.PhotoManager.resizeImageHeight(myImage, MaxHeightImage);
            }

            if (myImage.Width > MaxWidthImage)
            {
                myImage = Annunci.PhotoManager.resizeImageWidth(myImage, MaxWidthImage);
            }

            //Salvo l'immagine originale su file system
            string pathFile;
            pathFile = folder + System.IO.Path.GetFileName(MyFile.FileName);
            myImage.Save(Server.MapPath(pathFile));

            //'creo la thumbnail con altezza massima di 120px ... mantenendo le proporzioni 
            System.Drawing.Image thumbnail;
            thumbnail = Annunci.PhotoManager.resizeImageHeight(myImage, HeightThumbnail);
            pathFile = folder + "thumbnail_" + System.IO.Path.GetFileName(MyFile.FileName);
            thumbnail.Save(Server.MapPath(pathFile));


            //'inserimento su DB!
            Annunci.PhotoManager m = new Annunci.PhotoManager("immobiliare");

            try
            {
                m.mOpenConnection();

                //If _isPlanimetria Then
                //    _manager.insertPhoto(folder & IO.Path.GetFileName(AsyncFileUpload1.FileName), "PLANIMETRIA", _externalId, CType(Session("SessionData"), MyManager.SessionData).getUserId)
                //Else
                //    _manager.insertPhoto(folder & IO.Path.GetFileName(AsyncFileUpload1.FileName), "", _externalId, CType(Session("SessionData"), MyManager.SessionData).getUserId)
                //End If

                //m.insertPhoto(folder + System.IO.Path.GetFileName(MyFile.FileName), "", (long)annuncioId, (User.Identity as MyUsers.MyCustomIdentity).UserId);

                m.insertPhoto(folder + System.IO.Path.GetFileName(MyFile.FileName), "", (long)annuncioId, MySessionData.UserId);


            }
            finally
            {
                m.mCloseConnection();
            }





            model.esito = "Success";
            model.messaggio = "Operazione conlusa con successo";
            return Json(model);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetContatore(long? annuncioId)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "ResetContatore");
            }

            try
            {
                manager.mOpenConnection();
                manager.resetContatoreParziale((long)annuncioId);
            }
            finally
            {
                manager.mCloseConnection();
            }

            Models.JsonMessageModel model = new Models.JsonMessageModel();
            model.esito = "Success";
            model.messaggio = "Operazione conlusa con successo";
            return Json(model);
        }


        [HttpPost]
        public JsonResult DeleteImage(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            Debug.WriteLine(String.Format("ExternalId: {0}", id));

            Annunci.PhotoManager m = new Annunci.PhotoManager(manager.mGetConnection());

            try
            {
                m.mOpenConnection();
                m.deletePhoto((long)id, Server.MapPath("~"));
            }
            finally
            {
                m.mCloseConnection();
            }


            MyWebApplication.Models.JsonMessageModel model = new Models.JsonMessageModel();
            model.esito = "Success";
            model.messaggio = "Operazione conlusa con successo";
            return Json(model);
        }

    }
}
