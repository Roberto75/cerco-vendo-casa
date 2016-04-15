using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace Annunci.Models
{
    public class Risposta
    {
        public long rispostaId;
        public DateTime dateAdded;
        public string testo;
        public long refRispostaId;
        
        public long userId;
        public string login;
        //mi serve per sapere se si tratta di un'agenzia o meno
        public long customerId;

        public long trattativaId;
        public long annuncioId;

        

        public Risposta(System.Data.DataRow row, long inAnnuncioId)
        {

            userId = long.Parse(row["user_id"].ToString());
            login = row["my_login"].ToString();
            customerId = (row["customer_id"] is DBNull) ? -1 : long.Parse(row["customer_id"].ToString());

            trattativaId = long.Parse(row["fk_trattativa_id"].ToString());
            
            //annuncioId = long.Parse(row["annuncio_id"].ToString());
            annuncioId = inAnnuncioId;
            dateAdded = (row["date_added"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["date_added"].ToString());

            testo = row["testo"].ToString();
            rispostaId = long.Parse(row["risposta_id"].ToString());

            Debug.WriteLine(row["fk_risposta_id"].ToString());
            refRispostaId = (row["fk_risposta_id"] is DBNull) ? -1 : long.Parse(row["fk_risposta_id"].ToString());



        
        }


    }
}
