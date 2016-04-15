using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Models
{
   public class SearchImmobili : Pagedlmmobili  
    {

       public enum EnumTempo { 
           Oggi = 1,
           UltimaSettimana = 2,
           UltimoMese = 3,
           Tutti = 4
       }

        public enum EnumTipoAnnuncio
        {
            Agenzia,
            Privato
        }


        public enum EnumTipoVista
        {
            Elenco,
            Mappa
        }


        public Immobile  filter = new Immobile();

       public EnumTempo Tempo { get; set; }
       public EnumTipoVista TipoVista { get; set; }
       public List<EnumTipoAnnuncio> TipoAnnuncio {get; set; }



       public int? prezzoMax { get; set; }
       public int? mqMin { get; set; }


       public SearchImmobili()
       {
           this.Sort = "ANNUNCIO.date_added";
           this.SortDir = "DESC";

           TipoAnnuncio = new List<EnumTipoAnnuncio>() ;
           TipoAnnuncio.Add(Models.SearchImmobili.EnumTipoAnnuncio.Privato);
           TipoAnnuncio.Add(Models.SearchImmobili.EnumTipoAnnuncio.Agenzia);

           TipoVista = EnumTipoVista.Elenco;
           Tempo = EnumTempo.Tutti;

           prezzoMax = null;
           mqMin = null;
           
       }
      

    }
}
