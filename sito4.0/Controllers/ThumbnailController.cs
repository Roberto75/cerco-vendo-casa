using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Diagnostics;

namespace MyWebApplication.Controllers
{
    public class ThumbnailController : Controller
    {
        //http://aspnet.html.it/script/vedi/973/miniature-delle-immagini-thumbnail/

        public ActionResult Image(string id)
        {
            if (id == null)
            {
                id = "";
            }

            //Debug.WriteLine("ThumbnailController: " + id);


            string strFileName = id;

            if (!strFileName.StartsWith("~/"))
            {
                strFileName = "~/" + strFileName;
            }

            if (!System.IO.File.Exists(Server.MapPath(strFileName)))
            {
                strFileName = "~/Content/Images/Shared/unavailable.jpg";
            }

            short width = 100;
            short height = 100;

            System.Drawing.Image thumbnail;
            thumbnail = Annunci.PhotoManager.getThumbnailImage(Server.MapPath(strFileName), width, height);

            System.IO.MemoryStream buffer = new System.IO.MemoryStream();

            if (strFileName.ToLower().EndsWith(".jpg") || strFileName.ToLower().EndsWith(".jpeg"))
            {
                thumbnail.Save(buffer, System.Drawing.Imaging.ImageFormat.Jpeg);
                return new FileContentResult(buffer.ToArray(), "image/jpeg");
            } if (strFileName.ToLower().EndsWith(".gif"))
            {
                thumbnail.Save(buffer, System.Drawing.Imaging.ImageFormat.Gif);
                return new FileContentResult(buffer.ToArray(), "image/gif");
            } if (strFileName.ToLower().EndsWith(".bmp"))
            {
                thumbnail.Save(buffer, System.Drawing.Imaging.ImageFormat.Bmp);
                return new FileContentResult(buffer.ToArray(), "image/bmp");
            } if (strFileName.ToLower().EndsWith(".png"))
            {
                thumbnail.Save(buffer, System.Drawing.Imaging.ImageFormat.Png);
                return new FileContentResult(buffer.ToArray(), "image/png");
            }
            else// di default !!!
                throw new MyManagerCSharp.MyException("Formato dell'immagine non riconosciuto!");
        }



        //    public ActionResult Details(long id)
        //    {
        //        //Debug.WriteLine(Request.UrlReferrer);






        //        //If dt.Rows.Count = 0 Then
        //        //    'pathPhoto = "../Photo/images/unavailable.jpg"
        //        //    btnImage.ImageUrl = "~/Photo/images/unavailable.jpg"
        //        //Else
        //        //    If dt.Rows(0)("path").ToString.StartsWith("http") Then
        //        //        btnImage.ImageUrl = dt.Rows(0)("path")
        //        //        btnImage.Width = 80
        //        //        btnImage.Height = 60
        //        //    Else
        //        //        btnImage.ImageUrl = "~/Photo/getThumbnailImage.aspx?width=80&height=60&FileName=" & dt.Rows(0)("path")
        //        //    End If
        //        //End If
        //        //btnImage.ToolTip = "Cerco Vendo Casa: " & dataRow.Row("provincia") & " " & dataRow.Row("categoria") & " " & dataRow.Row("tipo") & " " & indirizzo


        //}
    }
}
