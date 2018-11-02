using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Models
{
    public class ReplyModel
    {
        public long annuncioId { get; set; }
        public long trattativaId { get; set; }
        public long rispostaId { get; set; }
        public string replyTo { get; set; }
        public Annunci.Models.Immobile annuncio { get; set; }
        [AllowHtml]
        public string testo { get; set; }

    }
}