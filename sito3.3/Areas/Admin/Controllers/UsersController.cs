using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyManagerCSharp;
using MyUsers.Models;
using MyUsers;
using System.Diagnostics;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class UsersController : MyBaseController
    {

        private UserManager manager = new UserManager("utenti");


        public ActionResult Index(MyUsers.Models.SearchUsers model)
        {
            
            if (model.Sort == "login")
            {
                model.Sort = "my_login";
            }
            else if (model.Sort == "DateAdded")
            {
                model.Sort = "date_added";
            }
            else if (model.Sort == "DateLastLogin")
            {
                model.Sort = "date_last_login";
            }


            TryUpdateModel(model.filter, "filter");

            Debug.WriteLine(String.Format("Filtri di ricerca  Nome: {0} Email: {1}", model.filter.nome, model.filter.email));


            manager.mOpenConnection();
            try
            {
                manager.getList(model);
            }
            finally
            {
                manager.mCloseConnection();
            }



            return View(model);

        }







        //
        // GET: /Users/Details/5

        public ActionResult Details(long id = 0)
        {
            Models.MyUserModel model = new Models.MyUserModel();

            //id = 1;

            manager.mOpenConnection();
            try
            {
                model.Utente = manager.getUser(id);

                if (model.Utente == null)
                {
                    return HttpNotFound();
                }

                // manager.setProfilo(myuser);

                GroupManager groupManager = new GroupManager(manager.mGetConnection());
                groupManager.setGroups(model.Utente);

                CustomerManager c = new CustomerManager(manager.mGetConnection());
                c.set(model.Utente);

            }
            finally
            {
                manager.mCloseConnection();
            }
            return View(model);
        }

        //
        // GET: /Users/Create

        public ActionResult Create()
        {
            List<string> t = new List<string>  {
        "Brendan Enrick", 
        "Kevin Kuebler", 
        "Todd Ropog"
    };

            ViewBag.lista = new SelectList(t);





            //populateComboBox(null);

            return View();
        }

        //
        // POST: /Users/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MyUsers.Models.MyUser myuser)
        {
            string t = Request.Form["Gruppi"];
            Debug.WriteLine("Gruppi: " + t);

            string gruppiSelected = Request.Form["gruppiIDs"];
            Debug.WriteLine("gruppiIDs: " + gruppiSelected);


            //http://www.hanselman.com/blog/ASPNETWireFormatForModelBindingToArraysListsCollectionsDictionaries.aspx

            MyHelper.printRequest(Request);


            //if (myuser.Gruppi == null)
            //{
            //    ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            //}

            if (String.IsNullOrEmpty(gruppiSelected))
            {
                ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            }



            if (ModelState.IsValid)
            {
                long newId;
                manager.mOpenConnection();
                try
                {
                    newId = manager.insert(myuser);
                    if (newId != -1)
                    {
                        List<MyGroup> lista = new List<MyGroup>();

                        foreach (string id in gruppiSelected.Split(','))
                        {
                            lista.Add(new MyGroup(int.Parse(id)));
                        }

                        myuser.Gruppi = lista;


                        GroupManager groupManager = new GroupManager(manager.mGetConnection());
                        groupManager.addUser(myuser.Gruppi, newId);
                    }
                }
                finally
                {
                    manager.mCloseConnection();
                }

                return RedirectToAction("Index");
            }



            var error = ModelState.SelectMany(x => x.Value.Errors);


            foreach (var value in ModelState.Values)
            {

                foreach (var merror in value.Errors)
                {

                    //throw new Exception(merror.ErrorMessage, merror.Exception);
                    Debug.WriteLine(merror.ErrorMessage + merror.Exception);

                }

            }
            //populateComboBox(myuser);



            return View(myuser);
        }

        //
        // GET: /Users/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Models.MyUserModel model = new Models.MyUserModel();

            //CustomerManager m1 = new CustomerManager(manager.mGetConnection());

            //List<MyGroup> listaGruppi = null;
            //List<MyCustomer> listaClienti = null;
            //List<MyProfile> listaProfili = null;

            manager.mOpenConnection();
            try
            {
                model.Utente = manager.getUser(id);

                if (model.Utente == null)
                {
                    return HttpNotFound();
                }

                //listaGruppi = m.getList();
                //listaClienti = m1.getList();
                //listaProfili = manager.getProfileList();

                GroupManager groupManager = new GroupManager(manager.mGetConnection());
                groupManager.setGroups(model.Utente);


                if (model.Utente.Gruppi != null)
                {
                    model.Gruppi = new MultiSelectList(groupManager.getList(), "gruppoId", "nome", model.Utente.Gruppi.Select(x => x.gruppoId).ToArray());
                }
                else
                {
                    model.Gruppi = new MultiSelectList(groupManager.getList(), "gruppoId", "nome", null);
                }

            }
            finally
            {
                manager.mCloseConnection();
            }




            ////MultiSelectList sl = new MultiSelectList(listaGruppi.ToList(), "gruppoId", "nome", new int[] {2}  );


            //MultiSelectList sl = new MultiSelectList(listaGruppi.ToList(), "gruppoId", "nome", listaGruppi.ToList());
            //ViewBag.listaGruppi = sl;

            //ViewBag.listaGruppi = listaGruppi;

            //ViewBag.clienti = new SelectList(listaClienti.ToList(), "customerId", "ragioneSociale");

            //ViewBag.profili = new SelectList(listaProfili.ToList(), "profileId", "nome");


            return View(model);
        }

        //
        // POST: /Users/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MyUserModel model)
        {
            MyHelper.printRequest(Request);

            //if (myuser.Gruppi == null)
            //{
            //    ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            //}

            string gruppiSelected = Request.Form["gruppiIDs"];
            Debug.WriteLine("gruppiIDs: " + gruppiSelected);

            if (String.IsNullOrEmpty(gruppiSelected))
            {
                ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            }



            if (ModelState.IsValid)
            {
                bool esito;
                manager.mOpenConnection();
                try
                {
                    //TODO: da modificare 02/02/2014
                    model.Utente.isEnabled = true;

                    esito = manager.update(model.Utente);

                    if (esito)
                    {
                        List<MyGroup> lista = new List<MyGroup>();

                        foreach (string id in gruppiSelected.Split(','))
                        {
                            lista.Add(new MyGroup(int.Parse(id)));
                        }

                        model.Utente.Gruppi = lista;

                        GroupManager groupManager = new GroupManager(manager.mGetConnection());
                        groupManager.update(model.Utente.Gruppi, (long) model.Utente.userId);
                    }
                }
                finally
                {
                    manager.mCloseConnection();
                }
                return RedirectToAction("Details", "Users", new { id = model.Utente.userId });
            }





            var error = ModelState.SelectMany(x => x.Value.Errors);


            foreach (var value in ModelState.Values)
            {

                foreach (var merror in value.Errors)
                {

                    //throw new Exception(merror.ErrorMessage, merror.Exception);
                    Debug.WriteLine(merror.ErrorMessage + merror.Exception);

                }

            }

            //manager.mOpenConnection();

            //populateComboBox(myuser);

            return View(model);
        }

        //
        // GET: /Users/Delete/5

        public ActionResult Delete(long id = 0)
        {


            MyUser myuser = null;
            manager.mOpenConnection();
            try
            {
                myuser = manager.getUser(id);
            }
            finally
            {
                manager.mCloseConnection();
            }


            if (myuser == null)
            {
                return HttpNotFound();
            }


            int numeroAnnunci = 0;
            Annunci.ImmobiliareManager immobiliareManager = new Annunci.ImmobiliareManager("immobiliare");
            try
            {
                immobiliareManager.mOpenConnection();
                numeroAnnunci = immobiliareManager.countAnnunci((long)id);
            }
            finally
            {
                immobiliareManager.mCloseConnection();
            }


            ViewBag.numeroAnnunci = numeroAnnunci;

            return View(myuser);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long? id)
        {

            if (id == null)
            {
                return HttpNotFound();
            }


            bool esito = false;

            Annunci.ImmobiliareManager immobiliareManager = new Annunci.ImmobiliareManager("immobiliare");
            try
            {
                immobiliareManager.mOpenConnection();
                esito = immobiliareManager.deleteUser((long)id, Server.MapPath("~"));
            }
            finally
            {
                immobiliareManager.mCloseConnection();
            }



            //Rutigliano 07/03/2014 potrei avere un utente che sta registato in Utenti e non su Immobiliare!
            //if (esito)
            //{

            manager.mOpenConnection();
            try
            {
                esito = manager.delete((long)id);

            }
            finally
            {
                manager.mCloseConnection();
            }
            //}

            return RedirectToAction("Index");
        }





        private void populateComboBoxOLD(MyUser myuser)
        {
            GroupManager m = new GroupManager(manager.mGetConnection());
            CustomerManager customerManager = new CustomerManager(manager.mGetConnection());

            m.mOpenConnection();

            List<MyGroup> listaGruppi;
            List<MyCustomer> listaClienti;
            List<MyProfile> listaProfili;

            try
            {
                listaGruppi = m.getList();
                listaClienti = customerManager.getList();
                listaProfili = manager.getProfili ();
            }
            finally
            {
                m.mCloseConnection();
            }


            //MultiSelectList sl = new MultiSelectList(listaGruppi.ToList(), "gruppoId", "nome");
            //ViewBag.listaGruppi = sl;
            //ViewBag.Gruppi = listaGruppi.ToList();
            //ViewBag.listaClienti = new SelectList(listaClienti.ToList(), "customerId", "ragioneSociale");

            if (myuser == null)
            {
                ViewBag.listaClienti = listaClienti.OrderBy(p => p.ragioneSociale).Select(p => new SelectListItem { Selected = false, Text = p.ragioneSociale, Value = p.customerId.ToString() });
                //ViewBag.customerId = new SelectList(listaClienti, "customerId", "ragioneSociale");
                ViewBag.Gruppi = new MultiSelectList(listaGruppi, "gruppoId", "nome");
            }
            else
            {
                ViewBag.listaClienti = listaClienti.OrderBy(p => p.ragioneSociale).Select(p => new SelectListItem { Selected = (p.customerId == myuser.customerId), Text = p.ragioneSociale, Value = p.customerId.ToString() });
                //ViewBag.customerId = new SelectList(db.Clienti, "customerId", "ragioneSociale", myuser.customerId);

                MultiSelectList msl;
                if (myuser.Gruppi != null)
                {
                    msl = new MultiSelectList(listaGruppi, "gruppoId", "nome", myuser.Gruppi.Select(x => x.gruppoId).ToArray());
                }
                else
                {
                    msl = new MultiSelectList(listaGruppi, "gruppoId", "nome", null);
                }

                ViewBag.Gruppi = msl;

                // ViewBag.Gruppi = new MultiSelectList(listaGruppi, "gruppoId", "nome", myuser.Gruppi);
            }
            ViewBag.listaProfili = new SelectList(listaProfili.ToList(), "profileId", "nome");

        }





        public ActionResult AutoCompleteLogin(string value)
        {
            Debug.WriteLine("AutoCompleteLogin: " + value);

            List<MyManagerCSharp.Models.MyItem> risultato;

            manager.mOpenConnection();

            try
            {
                risultato = manager.getAutoCompleteLogin(value);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return Json(risultato, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            //db.Dispose();
            base.Dispose(disposing);
        }
    }
}