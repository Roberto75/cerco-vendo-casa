using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Text;

using System.Diagnostics;

public static class MyHelper
{



    public static string decodeSiNo(int? value)
    {
        if (value == null || value == -1)
        {
            return "N/A";
        }

        if (value == 0)
        {
            return "No";
        }

        if (value == 1)
        {
            return "Si";
        }
        return "ERRORE: decodeSiNo value = " + value;
    }


    public static HtmlString getTitoloImmobile(Annunci.Models.Immobile immobile)
    {

        string temp;


        temp = String.Format("{0} {1}", immobile.categoria, immobile.immobile);


        HtmlString html;
        html = new HtmlString(temp);

        return html;
    }



    #region "Decode NULL"



    public static HtmlString decodeNull(DateTime? valore)
    {

        if (valore == null || valore == DateTime.MinValue)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.ToString());
    }

    public static HtmlString decodeNull(bool? valore)
    {

        if (valore == null)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.ToString());
    }


    public static HtmlString decodeNull(string valore)
    {

        if (String.IsNullOrEmpty(valore))
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore);
    }

    public static HtmlString decodeNull(decimal? valore)
    {

        if (valore == null)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.Value.ToString());
    }


    public static HtmlString decodeNull(int? valore)
    {

        if (valore == null)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.Value.ToString());
    }


    public static HtmlString decodeEnum(string value)
    {
        if (String.IsNullOrEmpty(value) || (value == "0") || (value == "Undefined"))
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(value.Replace("_", " "));
    }

    #endregion




    public static HtmlString decodeNumeroStanze(int? value)
    {
        if (value == null)
        {
            return new HtmlString("N/A");
        }

        if (value == 6)
        {
            return new HtmlString("+ di 5");
        }

        return new HtmlString(value.ToString());
    }




    public static HtmlString getLocalitaImmobileVerticale(Annunci.Models.Immobile immobile)
    {

        if (immobile == null)
        {
            return null;
        }

        string temp;


        temp = String.Format("<p>{0}</p>", immobile.regione);

        if (immobile.provincia != immobile.comune)
        {
            //temp += " - " + immobile.provincia + " - " + immobile.comune;
            temp += String.Format("<p>{0}</p>", immobile.provincia);
            temp += String.Format("<p>{0}</p>", immobile.comune);
        }
        else
        {
            //temp += " - " + immobile.comune;
            temp += String.Format("<p>{0}</p>", immobile.comune);
        }


        HtmlString html;
        html = new HtmlString(temp);

        return html;
    }



    public static HtmlString getLocalitaImmobileOrizzontale(Annunci.Models.Immobile immobile)
    {

        if (immobile == null)
        {
            return null;
        }

        string temp;


        temp = String.Format("{0}", immobile.regione);

        if (immobile.provincia != immobile.comune)
        {
            //temp += " - " + immobile.provincia + " - " + immobile.comune;
            temp += String.Format(" - {0}", immobile.provincia);
            temp += String.Format(" - {0}", immobile.comune);
        }
        else
        {
            //temp += " - " + immobile.comune;
            temp += String.Format(" - {0}", immobile.comune);
        }


        HtmlString html;
        html = new HtmlString(temp);

        return html;
    }



    public static HtmlString getPhotoImmobile(long annuncioId)
    {

        //string pathImage;
        // pathImage = "~/Images/unavailable.jpg";
        //athImage = System.Web.Mvc.UrlHelper.GenerateUrl("id", "Image", "Thumbnail", "sssss", 
        //pathImage = System.Web.Mvc.UrlHelper.GenerateContentUrl(pathImage, Context);



        string pathImage = "";
        string toolTip;

        Annunci.ImmobiliareManager manager = new Annunci.ImmobiliareManager("immobiliare");
        try
        {
            manager.mOpenConnection();

            System.Data.DataTable dt;

            dt = manager.getPhoto(annuncioId);

            if (dt.Rows.Count == 0)
            {
                pathImage = "/Content/Images/Shared/unavailable.jpg";
                toolTip = "Immagine non disponibile";
            }
            else
            {
                pathImage = dt.Rows[0]["path"].ToString();

                if (!pathImage.StartsWith("http"))
                {
                    pathImage = String.Format("/Thumbnail/Image?id={0}", HttpUtility.UrlEncode(pathImage));
                }

                toolTip = String.Format("Sono disponibili {0} immagini", dt.Rows.Count);
            }
        }
        finally
        {

            manager.mCloseConnection();
        }


        // pathImage = System.Web.Mvc.UrlHelper.GenerateContentUrl(pathImage, Context);



        string temp;
        temp = String.Format("<a href=\"\\Immobiliare\\Details\\{2}\"><img src=\"{0}\" title=\"{1}\" height=\"80\" width=\"80\" /></a>", pathImage, toolTip, annuncioId);





        return new HtmlString(temp);
    }


    public static HtmlString getProfileImage(string ServerRootPath, string login, long customerId)
    {
        return MyHelper.getProfileImage(ServerRootPath, login, customerId, -1);
    }


    public static HtmlString getProfileImage(string ServerRootPath, string login, long customerId, long annuncioId)
    {

        string pathImage;
        pathImage = "/public/UserFiles/" + login + "/photo.gif";

        string temp;

        //System.Diagnostics.Debug.WriteLine(String.Format ("Login: {0} CustomerId: {1}", login , customerId ));

        if (!System.IO.File.Exists(ServerRootPath + pathImage))
        {

            if (customerId == -1)
            {
                pathImage = "/Content/Images/immobiliare/_privato.gif";
                //pathImage = "~/Content/themes/base/images/immobiliare/_privato.gif";
            }
            else
            {
                pathImage = "/Content/Images/immobiliare/_agenzia.gif";
                //pathImage = "~/Content/themes/base/images/immobiliare/_agenzia.gif";
            }
        }

        string toolTip;
        if (customerId == -1)
        {
            toolTip = "Utente privato: " + login;
        }
        else
        {
            toolTip = "Agenzia immobiliare: " + login;
        }

        //pathImage = System.Web.Mvc.UrlHelper.GenerateContentUrl(pathImage, Context);

        //temp = String.Format("<img src=\"{0}\" title=\"{1}\" />", pathImage, toolTip);


        temp = String.Format("<img src=\"{0}\" title=\"{1}\" height=\"80\" width=\"80\" /></a>", pathImage, toolTip);

        if (annuncioId != -1)
        {

            temp = String.Format("<a href=\"\\Immobiliare\\Details\\{0}\">{1}</a>", annuncioId, temp);
        }


        return new HtmlString(temp);
    }


    public static HtmlString getDatiImmobile(Annunci.Models.Immobile immobile)
    {

        string temp = "";

        temp += "<div style=\"min-width:250px;\" >";

        temp += "<div style=\"width:100%;text-align:center;\" ><b>" + immobile.immobile.ToString() + "</b></div>";


        string style1 = " style=\"float:left;width:50%;\" ";
        string style2 = " style=\"margin-left:50%;\" ";
        //string style2 = "";
        //temp += "<table cellpadding=\"0\" cellspacing =\"0\" width=\"100%\" style=\"padding-bottom:3px;\">";

        //temp += String.Format("<tr><td style=\"width:50%\"> {0} </td><td> {1} </td></tr>", ((piano == null || piano == -1) ? "" : "Piano: " + piano), (salone == Annunci.Models.Immobile.TipoSalone.Undefined) ? "" : "Salone: " + salone.ToString());


        //temp += "</table>";


        //temp += String.Format("<div width=\"50%\">{0}</div><div width=\"50%\">{1}</div>", ((immobile.piano == null || immobile.piano == -1) ? "" : "Piano: " + immobile.piano), (immobile.salone == Annunci.Models.Immobile.TipoSalone.Undefined) ? "" : "Salone: " + immobile.salone.ToString());
        //temp += String.Format("<div>{0}</div><div>{1}</div>", ((ca == null || piano == -1) ? "" : "Piano: " + piano), (salone == Annunci.Models.Immobile.TipoSalone.Undefined) ? "" : "Salone: " + salone.ToString());


        temp += String.Format("<div {0} > {2}</div>        <div {1} >Salone: {3}</div>", style1, style2, (immobile.piano == null) ? "" : "Piano: " + decodeNull(immobile.piano), decodeEnum(immobile.salone.ToString()));
        temp += String.Format("<div {0} >Posto auto: {2}</div>   <div {1} >Box: {3}</div>", style1, style2, decodeEnum(immobile.postoAuto.ToString()), decodeEnum(immobile.box.ToString()));

        //temp += String.Format("<div width=\"50%\" style=\"float:left\">{0}</div><div width=\"50%\" style=\"margin-left:5px\">{1}</div>", "A: AAA", "B: BBB");
        //temp += String.Format("<div width=\"50%\" style=\"float:left\">{0}</div><div width=\"50%\">{1}</div>", "C: ccc", "d: ddd");


        temp += "</div>";

        HtmlString html;
        html = new HtmlString(temp);

        return html;
    }

    //public static HtmlString CheckboxListForEnum<T>(this HtmlHelper html, string name, T modelItems) where T : struct
    //{
    //    StringBuilder sb = new StringBuilder();

    //    foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
    //    {
    //        TagBuilder builder = new TagBuilder("input");
    //        long targetValue = Convert.ToInt64(item);
    //        long flagValue = Convert.ToInt64(modelItems);

    //        if ((targetValue & flagValue) == targetValue)
    //            builder.MergeAttribute("checked", "checked");

    //        builder.MergeAttribute("type", "checkbox");
    //        builder.MergeAttribute("value", item.ToString());
    //        builder.MergeAttribute("name", name);
    //        builder.InnerHtml = item.ToString();

    //        sb.Append(builder.ToString(TagRenderMode.Normal));
    //    }

    //    return new HtmlString(sb.ToString());
    //}



    //public static SelectList ToSelectList(this Enum enumObj)
    //{

    //    foreach (var value in Enum.GetValues(typeof(Enum)))
    //    {
    //        Debug.WriteLine(String.Format("{0}", value));
    //    }



    //    return new SelectList(null, "Id", "Name", enumObj);
    //}





    //public static IEnumerable<SelectListItem > ToSelectList<TEnum>(this TEnum listOfValues,  string currentValue)
    //{

    //    Debug.WriteLine(String.Format("currentValue: {0}", currentValue));

    //    foreach (var value in Enum.GetValues(typeof(TEnum)))
    //    {
    //        Debug.WriteLine(String.Format("{0}", value));
    //    }





    ////    var result =
    ////from e in Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
    ////select new
    ////{
    ////    Id = (int)Enum.Parse(typeof(TEnum), e.ToString()),
    ////    Name = e.ToString()
    ////};


    //    List <SelectListItem  > risultato = new List<SelectListItem> ();

    //    risultato.Add (new SelectListItem (){ Text = "---", Value = "" });

    //  //  risultato.AddRange(from e in Enum.GetValues(typeof(TEnum)).Cast<TEnum>() select new SelectListItem { Value = ((int)Enum.Parse(typeof(TEnum), e.ToString()) ).ToString() , Text = e.ToString() });

    //    bool selected;
    //    foreach (var e in Enum.GetValues(typeof(TEnum)))
    //    {
    //        Debug.WriteLine(String.Format("{0}", e));
    //        selected = false;
    //        if (e.ToString().Equals(currentValue))
    //        {
    //            selected = true;
    //        }


    //        risultato.Add(new SelectListItem() { Value = ((int)Enum.Parse(typeof(TEnum), e.ToString())).ToString(), Text = e.ToString(), Selected = selected });

    //    }

    //  //  SelectListItem e = new SelectListItem ();
    //    //e.Selected = 

    //    //if (String.IsNullOrEmpty(currentValue))
    //    //{
    //    //    return new SelectList(risultato, "Value", "Text", null );
    //    //}

    //   // return new SelectList(risultato, "Value", "Text", ((int) Enum.Parse( typeof(TEnum),  currentValue.ToString())).ToString() );
    //    //return new SelectList(risultato, "Value", "Text", );
    //    //return new SelectList(risultato, "Value", "Text", "2");
    //   // return new SelectList(risultato, "Value", "Text");
    //    return risultato;
    //}




    #region "Combo Box"

    public static HtmlString getComboEnum<TEnum>(TEnum allValues, string selectedValue, string nome, bool isRequired)
    {

        //Debug.WriteLine(String.Format("selectedValue: {0}", selectedValue));

        //foreach (var value in Enum.GetValues(typeof(TEnum)))
        //{
        //    Debug.WriteLine(String.Format("{0}", value));
        //}


        string temp;

        string required = "";

        if (isRequired)
        {
            required = "required=\"required\"";
        }

        //data-inline=\"true\" 

        temp = String.Format("<select id=\"{0}\" name=\"{1}\"    class=\"form-control\" {2}  >", nome.Replace(".", "_"), nome, required);

        temp += "<option value=\"\">---</option>";

        foreach (var e in Enum.GetValues(typeof(TEnum)))
        {

            if (e.ToString().Equals("Undefined"))
            {
                continue;
            }

            if (selectedValue.Equals(e.ToString()))
            {
                temp += String.Format("<option value=\"{0}\"  selected=\"selected\"  >{1}</option>", ((int)Enum.Parse(typeof(TEnum), e.ToString())).ToString(), e.ToString().Replace("_", " "));
            }
            else
            {
                temp += String.Format("<option value=\"{0}\">{1}</option>", ((int)Enum.Parse(typeof(TEnum), e.ToString())).ToString(), e.ToString().Replace("_", " "));
            }
        }


        temp += "</select>";

        return new HtmlString(temp);
    }






    public static HtmlString getComboNumeroStanze(string selectedValue, string nome, bool isRequired)
    {

        string temp;

        string required = "";

        if (isRequired)
        {
            required = "required=\"required\"";
        }


        temp = String.Format("<select id=\"{0}\" name=\"{1}\" class=\"form-control mb-3\"   {2}  >", nome.Replace(".", "_"), nome, required);

        temp += "<option value=\"\" " + (String.IsNullOrEmpty(selectedValue) ? "selected=\"selected\"" : "") + " >---</option>";

        temp += "<option value=\"0\" " + ((selectedValue == "0") ? "selected=\"selected\"" : "") + " >0</option>";
        temp += "<option value=\"1\" " + ((selectedValue == "1") ? "selected=\"selected\"" : "") + " >1</option>";
        temp += "<option value=\"2\" " + ((selectedValue == "2") ? "selected=\"selected\"" : "") + " >2</option>";
        temp += "<option value=\"3\" " + ((selectedValue == "3") ? "selected=\"selected\"" : "") + " >3</option>";
        temp += "<option value=\"4\" " + ((selectedValue == "4") ? "selected=\"selected\"" : "") + " >4</option>";
        temp += "<option value=\"5\" " + ((selectedValue == "5") ? "selected=\"selected\"" : "") + " >5</option>";
        temp += "<option value=\"6\" " + ((selectedValue == "6") ? "selected=\"selected\"" : "") + " >+ di 5</option>";

        temp += "</select>";

        return new HtmlString(temp);
    }


    public static HtmlString getComboSiNo(string selectedValue, string id, string nome, bool isRequired)
    {
        string temp;
        string required = "";

        if (isRequired)
        {
            required = "required=\"required\"";
        }

        temp = String.Format("<select id=\"{0}\" name=\"{1}\" class=\"form-control mb-3\"   {2}  >", id.Replace(".", "_"), nome, required);

        temp += "<option value=\"-1\" " + (String.IsNullOrEmpty(selectedValue) ? "selected=\"selected\"" : "") + " >---</option>";
        temp += "<option value=\"1\" " + ((selectedValue == "1") ? "selected=\"selected\"" : "") + " >Si</option>";
        temp += "<option value=\"0\" " + ((selectedValue == "0") ? "selected=\"selected\"" : "") + " >No</option>";

        temp += "</select>";

        return new HtmlString(temp);
    }


    public static HtmlString getComboTipoImmobile(Annunci.Models.Immobile.TipoImmobile? value)
    {
        string temp;

        temp = "<select id=\"immobile\" name=\"immobile\"   data-mini=\"true\" data-inline=\"true\"  >";

        temp += "<option value=\"\">---</option>";

        foreach (Annunci.Models.Immobile.TipoImmobile e in Enum.GetValues(typeof(Annunci.Models.Immobile.TipoImmobile)))
        {

            if (value == e)
            {
                temp += String.Format("<option value=\"{0}\"  selected=\"selected\"  >{1}</option>", e, (Annunci.Models.Immobile.TipoImmobile)e);
            }
            else
            {
                temp += String.Format("<option value=\"{0}\">{1}</option>", e, (Annunci.Models.Immobile.TipoImmobile)e);
            }
        }


        temp += "</select>";

        return new HtmlString(temp);
    }



    #endregion




    public static void printRequest(System.Web.HttpRequestBase request)
    {

        foreach (string k in request.Form.AllKeys)
        {
            Debug.WriteLine(String.Format("Key: {0} \t Value: {1}", k, request[k]));
        }

    }
}
