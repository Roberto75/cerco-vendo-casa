using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace MyWebApplication.Controllers
{
    public class RegioneProvinciaComuneController : Controller
    {
        //
        // GET: /RegioneProvinciaComune/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetProvince(long regioneId)
        {
            MyManagerCSharp.RegioniProvinceComuniManager m = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");
            m.mOpenConnection();

            System.Data.DataTable dt = null;

            try
            {

                //  regioneId = int.Parse (m.getRegioneByLabel(regione).Rows[0]["id"].ToString()) ; 

                dt = m.getProvince(regioneId);
            }
            finally
            {
                m.mCloseConnection();
            }

            return Json(dt.AsEnumerable().Select(x => new { value = x.Field<string>("Id"), text = x.Field<string>("valore") }), JsonRequestBehavior.AllowGet);

            //return Json(Enumerable.Range(1, 12).Select(x => new { value = x, text = x }),  JsonRequestBehavior.AllowGet );
        }


        public ActionResult GetComuni(string provinciaId)
        {
            MyManagerCSharp.RegioniProvinceComuniManager m = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");
            m.mOpenConnection();

            System.Data.DataTable dt = null;

            try
            {


                dt = m.getComuni(provinciaId);
            }
            finally
            {
                m.mCloseConnection();
            }

            return Json(dt.AsEnumerable().Select(x => new { value = x.Field<string>("Id"), text = x.Field<string>("valore") }), JsonRequestBehavior.AllowGet);

        }
    }
}
