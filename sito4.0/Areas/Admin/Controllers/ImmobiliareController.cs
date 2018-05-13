using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class ImmobiliareController : MyBaseController
    {

        private Annunci.AnnunciAdminManager manager = new Annunci.AnnunciAdminManager("immobiliare");

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Users(Annunci.Models.SearchUsers model)
        {
            int totalRecords;

            TryUpdateModel(model.filter, "filter");

            manager.mOpenConnection();
            try
            {

                if (Request["tipo"] == null || String.IsNullOrEmpty(Request["tipo"].ToString()))
                {
                    model.tipo = -1;
                }
                else
                {

                    model.tipo = int.Parse(Request["tipo"].ToString());
                }

                Debug.WriteLine(String.Format("Filtri di ricerca Tipo: {2} Nome: {0} Email: {1}", Request["filter.nome"], Request["filter.email"], model.tipo));

                if (model.Sort == "login")
                {
                    model.Sort = "my_login";
                }
                else if (model.Sort == "DateAdded")
                {
                    model.Sort = "date_added";

                }
                else if (model.Sort == "DateStato")
                {
                    model.Sort = "date_stato";
                }
                else if (model.Sort == "externlId")
                {
                    model.Sort = "external_id";
                }


                model.Utenti = manager.getUsers(out totalRecords, model.filter, model.PageSize, model.PageNumber, model.Sort, model.SortDir);

                model.TotalRows = totalRecords;
            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(model);
        }


        public ActionResult Details(long id)
        {
            Debug.WriteLine("Utente: " + id);

            MyWebApplication.Areas.Admin.Models.MyUserImmobiliareModel model;
            model = new Models.MyUserImmobiliareModel();


            manager.mOpenConnection();
            try
            {
                model.Utente = manager.getUser(id);

                if (model.Utente == null)
                {
                    return HttpNotFound();
                }


                model.ContaAnnunciByStato = manager.countAnnunciByStato(id);
                model.ContaTrattativeByStato = manager.countTrattativeByStato(id);

            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(model);
        }


        public ActionResult Edit(long id)
        {
            Debug.WriteLine("Utente: " + id);

            Annunci.Models.MyUser model;

            manager.mOpenConnection();
            try
            {
                model = manager.getUser(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Annunci.Models.MyUser model)
        {
            if (ModelState.IsValid)
            {
                bool esito;
                manager.mOpenConnection();
                try
                {
                    Annunci.Models.MyUser checkIfExists;

                    checkIfExists = manager.getUtenteByExternalId(model.externalId, model.sourceId, (long)model.userId);

                    if (checkIfExists != null)
                    {
                        ModelState.AddModelError(string.Empty, "Esiste già un utente con questo external ID : " + model.externalId + " che appartiene alla source: " + model.sourceId);
                        return View(model);
                    }


                    esito = manager.update(model);
                    return RedirectToAction("Details", "Immobiliare", new { id = model.userId });
                }
                finally
                {
                    manager.mCloseConnection();
                }

            }

            return View(model);
        }

    }
}
