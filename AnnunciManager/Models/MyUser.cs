using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Models
{
    public class MyUser : MyUsers.Models.MyUser
    {
        public string externalId { get; set; }
        public Models.Immobile.TipoSourceId? sourceId { get; set; }
        public string nota { get; set; }
        public DateTime? dateStato { get; set; }
        public Models.Immobile.StatoUtente? statoId { get; set; }

        public System.Collections.Hashtable numeroDiAnnunciByStato { get; set; }

        public int DaysStatus
        {
            get
            {
                if (dateStato == null)
                {
                    return 0;
                }

                TimeSpan span = DateTime.Now - (DateTime) dateStato;
                return span.Days;
            }
        }



        public MyUser()
        {
        }

        public MyUser(System.Data.DataRow row)
        {
            userId = long.Parse(row["user_id"].ToString());
            login = row["my_login"].ToString();
            nome = row["nome"].ToString();
            cognome = row["cognome"].ToString();
            email = row["email"].ToString();

            externalId = row["external_id"].ToString();
            //sourceId = (row["source.source_id"] is DBNull) ? null : int.Parse(row["source.source_id"].ToString());
            if (row["source.source_id"] is DBNull)
            {
                sourceId = null;
            }
            else
            {
                //sourceId = int.Parse(row["source.source_id"].ToString());
                sourceId = (Models.Immobile.TipoSourceId)Enum.Parse(typeof(Models.Immobile.TipoSourceId), row["source.source_id"].ToString());
            }


            if (row["date_stato"] is DBNull)
            {
                dateStato = null;
            }
            else
            {
                dateStato = DateTime.Parse(row["date_stato"].ToString());
            }


            if (row["stato_id"] is DBNull)
            {
                statoId = null;
            }
            else
            {
                // statoId = int.Parse(row["stato_id"].ToString());
                statoId = (Models.Immobile.StatoUtente)Enum.Parse(typeof(Models.Immobile.StatoUtente), row["stato_id"].ToString());
            }


            nota = row["nota"].ToString();
        }

    }
}
