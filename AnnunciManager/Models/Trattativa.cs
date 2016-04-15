using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Models
{
    public class Trattativa
    {


        public enum StatoTrattativa
        {
            Undefined = -1,
            Attiva = 1,
            AnnuncioRimosso = 2,
            NonPiuDiIntertesse = 3,
            TerminataConSuccesso = 4,
            Altro = 5
        }

        
        //Attiva = 1
        //NonPiuDiInteresse
        //AnnuncioRimosso
        //TerminataConSuccesso
        //TerminataSenzaSuccesso
        //TerminataConFrode
        //Altro


        public long trattativaId;
        public DateTime dateAdded;
        public long annuncioId;
        public long userId;
        public string login;

        public DateTime dateDeleted;
        public DateTime dateDeletedOwner;

        public StatoTrattativa stato;
        public decimal prezzo { get; set; }
        public Models.Immobile.TipoImmobile tipo;
        public Models.Immobile.Categorie categoria;

        public IEnumerable<Risposta> risposte;

        public Trattativa() { }

        public Trattativa(System.Data.DataRow row)
        {

            userId = long.Parse(row["user_id"].ToString());
            login = row["my_login"].ToString();
            trattativaId = long.Parse(row["trattativa_id"].ToString());
            annuncioId = long.Parse(row["annuncio_id"].ToString());
            dateAdded = (row["date_added"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["date_added"].ToString());
            stato = (row["stato"] is DBNull) ? Models.Trattativa.StatoTrattativa.Undefined : (Models.Trattativa.StatoTrattativa)Enum.Parse(typeof(Models.Trattativa.StatoTrattativa), row["stato"].ToString());
            prezzo = (row["prezzo"] is DBNull) ? 0 : Decimal.Parse(row["prezzo"].ToString());
            tipo = (Models.Immobile.TipoImmobile)Enum.Parse(typeof(Models.Immobile.TipoImmobile), row["tipo"].ToString());
            categoria = (Models.Immobile.Categorie)int.Parse(row["categoria_id"].ToString());

        }

    }
}
