using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Models
{
    public class ReplyModel
    {
        public long annuncioId { get; set; }
        public long trattativaId { get; set; }
        public long rispostaId { get; set; }

        public Annunci.Models.Immobile annuncio { get; set; }
       
        public string testo { get; set; }

    }
}