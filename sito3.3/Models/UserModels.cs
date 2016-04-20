using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace MyWebApplication.Models
{
    //public class LogOnModel
    //{
    //    [Required]
    //    [Display(Name = "User name")]
    //    public string UserName { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }

    //    public bool RememberMe { get; set; }
    //}


    //http://shashankshetty.wordpress.com/2009/03/04/using-jsonresult-with-jquery-in-aspnet-mvc/
    public class JsonMessageModel
    {
        public string messaggio;
        public string esito;

    }


}