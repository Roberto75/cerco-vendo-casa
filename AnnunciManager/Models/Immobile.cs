using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci.Models
{
    public class Immobile
    {


        public enum SelectFileds
        {
            Lista,
            Full
        }

        public enum SiNo
        {
            No = 0,
            Si = 1
        }


          public enum StatoUtente
        {
            Undefined = -1,
            Active = 1,
            Test = 2,
            Disabled = 3
        }


        public enum Categorie
        {
            Affitto = 1000000,
            Scambio = 4000000,
            Acquisto = 2000000,
            Vendita = 3000000
        }



        public enum TipoImmobile
        {
            Appartamento = 1,
            Monolocale = 2,
            Attico = 3,
            Villa = 4,
            Casale = 5,
            Box = 6,
            Ufficio = 7,
            Negozio = 8,
            //aggiunti per RevoAgent
            Capannone = 9,
            Terreno = 10,
            Loft = 11,
            Mansarda = 12,
            Altro = 13,
        }

        public enum TipoCucina
        {
            Undefined = -1,
            No = 0,
            Angolo_cottura = 1,
            Semi_abitabile = 2,
            Abitabile = 3,
            Con_tinello = 4,
            Altro = 5
        }

        public enum TipoSalone
        {
            Undefined = -1,
            No = 0,
            Singolo = 1,
            Doppio = 2,
            Triplo = 3,
            Quadruplo = 4,
            Altro = 5
        }

        public enum TipoBoxAuto
        {
            Undefined = -1,
            No = 0,
            Singolo = 1,
            Doppio = 2,
            Triplo = 3,
            Altro = 4
        }

        public enum TipoOccupazione
        {
            Undefined = -1,
            Libero = 1,
            In_locazione = 2,
            Nuda_proprieta = 3
        }

        public enum TipoStatoImmobile
        {
            Undefined = -1,
            Ristrutturato = 1,
            Da_ristrutturare = 2,
            In_costruzione = 3
        }

        public enum TipoRiscaldamento
        {
            Undefined = -1,
            No = 0,
            Autonomo = 1,
            Centralizzato = 2,
            Centralizzato_ripartito = 3
        }

        public enum TipoPostoAuto
        {
            Undefined = -1,
            No = 0,
            Singolo_scoperto = 1,
            Singolo_coperto = 2,
            Doppio_scoperto = 3,
            Doppio_coperto = 4,
            Triplo_scoperto = 5,
            Triplo_coperto = 6,
            Altro = 7
        }


        public enum TipoClasseEnergetica
        {
            Undefined = -1,
            AA,
            A,
            B,
            C,
            D,
            E,
            F,
            G
        }




        public enum TipoSourceId
        {
            Undefined = -1,
            RevoAgent = 1
        }


        public long immobileId { get; set; }
        public DateTime dataInserimento { get; set; }

        public long customerId { get; set; }
        public long userId { get; set; }
        public string login { get; set; }

        public int MQ { get; set; }

        //nota/descizione
       public string nota { get; set; }


        // le imposto a null atlrimeti mi diventa un filtro obbligatorio nella RICERCA
        public Categorie? categoria { get; set; }

        //tipo di immobile :appartamento, casale, ...
        // le imposto a null atlrimeti mi diventa un filtro obbligatorio nella RICERCA
        public TipoImmobile? immobile { get; set; }

        public decimal prezzo { get; set; }


        public string regione { get; set; }
        public string provincia { get; set; }
        public string comune { get; set; }
        public int regioneId { get; set; }
        public string provinciaId { get; set; }
        public string comuneId { get; set; }


        public int? piano { get; set; }
        public int? pianiTotali { get; set; }
        public int? anno { get; set; }
        public TipoCucina? cucina { get; set; }
        public TipoSalone? salone { get; set; }

        public string indirizzo { get; set; }
        public string quartiere { get; set; }
        public string cap { get; set; }

        public int? camereDaLetto { get; set; }
        public int? camerette { get; set; }
        public int? bagni { get; set; }
        public int? terrazzi { get; set; }
        public int? balconi { get; set; }
        public int? giardinoMq { get; set; }
        public int? cantina { get; set; }
        public int? soffitta { get; set; }

        public TipoPostoAuto? postoAuto { get; set; }
        public TipoBoxAuto? box { get; set; }
        public TipoRiscaldamento? riscaldamento { get; set; }

        public TipoOccupazione? occupazione { get; set; }
        public TipoStatoImmobile? statoImmobile { get; set; }

        public decimal? speseCondominiali { get; set; }

        public int? climatizzato { get; set; }
        public int? ascensore { get; set; }
        public int? portiere { get; set; }

        public double latitude { get; set; }
        public double longitude { get; set; }

        public TipoClasseEnergetica? classeEnergetica { get; set; }


        public string externalId { get; set; }
        public TipoSourceId sourceId { get; set; }


        //21/03/2014
        public DateTime dateLastClick { get; set; }
        public DateTime dateStartClickParziale { get; set; }
        public long countClick { get; set; }
        public long countClickParziale { get; set; }


        public Immobile()
        {
            this.immobileId = -1;
        }

        public Immobile(System.Data.DataRow row, SelectFileds mode)
        {
            immobileId = long.Parse(row["annuncio_id"].ToString());
            prezzo = (row["prezzo"] is DBNull) ? 0 : Decimal.Parse(row["prezzo"].ToString());

            dataInserimento = (row["date_added"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["date_added"].ToString());

            //dataInserimento = DateTime.ParseExact(row["date_added"].ToString(), "dd-MM-yyyy", null);
            MQ = (row["MQ"] is DBNull) ? 0 : int.Parse(row["MQ"].ToString());

            userId = (row["user_id"] is DBNull) ? 0 : long.Parse(row["user_id"].ToString());

            login = (row["my_login"] is DBNull) ? "" : row["my_login"].ToString();

            categoria = (Models.Immobile.Categorie)int.Parse(row["categoria_id"].ToString());
            salone = (row["salone"] is DBNull) ? TipoSalone.Undefined : (Models.Immobile.TipoSalone)Enum.Parse(typeof(Models.Immobile.TipoSalone), row["salone"].ToString());

            immobile = (Models.Immobile.TipoImmobile)Enum.Parse(typeof(Models.Immobile.TipoImmobile), row["tipo"].ToString());

            customerId = (row["customer_id"] is DBNull) ? -1 : long.Parse(row["customer_id"].ToString());

            longitude = (row["longitude"] is DBNull) ? 0 : double.Parse(row["longitude"].ToString());
            latitude = (row["latitude"] is DBNull) ? 0 : double.Parse(row["latitude"].ToString());

            regione = (row["regione"] is DBNull) ? "" : row["regione"].ToString();
            provincia = (row["provincia"] is DBNull) ? "" : row["provincia"].ToString();
            comune = (row["comune"] is DBNull) ? "" : row["comune"].ToString();


            if (mode == SelectFileds.Full)
            {

                statoImmobile = (row["stato"] is DBNull) ? TipoStatoImmobile.Undefined : (Models.Immobile.TipoStatoImmobile)Enum.Parse(typeof(Models.Immobile.TipoStatoImmobile), row["stato"].ToString());
                occupazione = (row["occupazione"] is DBNull) ? TipoOccupazione.Undefined : (Models.Immobile.TipoOccupazione)Enum.Parse(typeof(Models.Immobile.TipoOccupazione), row["occupazione"].ToString());
                classeEnergetica = (row["classe_energetica"] is DBNull) ? TipoClasseEnergetica.Undefined : (Models.Immobile.TipoClasseEnergetica)Enum.Parse(typeof(Models.Immobile.TipoClasseEnergetica), row["classe_energetica"].ToString());

                cucina = (row["cucina"] is DBNull) ? TipoCucina.Undefined : (Models.Immobile.TipoCucina)Enum.Parse(typeof(Models.Immobile.TipoCucina), row["cucina"].ToString());
                postoAuto = (row["posto_auto"] is DBNull) ? TipoPostoAuto.Undefined : (Models.Immobile.TipoPostoAuto)Enum.Parse(typeof(Models.Immobile.TipoPostoAuto), row["posto_auto"].ToString());
                box = (row["box"] is DBNull) ? TipoBoxAuto.Undefined : (Models.Immobile.TipoBoxAuto)Enum.Parse(typeof(Models.Immobile.TipoBoxAuto), row["box"].ToString());
                riscaldamento = (row["riscaldamento"] is DBNull) ? TipoRiscaldamento.Undefined : (Models.Immobile.TipoRiscaldamento)Enum.Parse(typeof(Models.Immobile.TipoRiscaldamento), row["riscaldamento"].ToString());
                
                anno = (row["anno"] is DBNull) ? 0 : int.Parse(row["anno"].ToString());
                piano = (row["piano"] is DBNull) ? 0 : int.Parse(row["piano"].ToString());
                pianiTotali = (row["piani_totali"] is DBNull) ? 0 : int.Parse(row["piani_totali"].ToString());

                indirizzo = (row["indirizzo"] is DBNull) ? "" : row["indirizzo"].ToString();
                cap = (row["cap"] is DBNull) ? "" : row["cap"].ToString();
                quartiere = (row["quartiere"] is DBNull) ? "" : row["quartiere"].ToString();

                regioneId = (row["regione_id"] is DBNull) ? -1 : int.Parse(row["regione_id"].ToString());
                provinciaId = (row["provincia_id"] is DBNull) ? "" : row["provincia_id"].ToString();
                comuneId = (row["comune_id"] is DBNull) ? "" : row["comune_id"].ToString();

                ascensore = (row["ascensore"] is DBNull) ? -1 : int.Parse(row["ascensore"].ToString());

                nota = (row["DESCRIZIONE"] is DBNull) ? "" : row["DESCRIZIONE"].ToString();


                dateLastClick = (row["DATE_LAST_CLICK"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["DATE_LAST_CLICK"].ToString());
                dateStartClickParziale = (row["date_start_click_parziale"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["date_start_click_parziale"].ToString());
                countClick = long.Parse(row["COUNT_CLICK"].ToString());
                countClickParziale = long.Parse(row["COUNT_CLICK_PARZIALE"].ToString());
            }

        }
    }
}
