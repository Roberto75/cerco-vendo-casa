using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MyUsers;

namespace Annunci
{
    public class ImmobiliareManager : MyManagerCSharp.ManagerDB
    {


        //public enum Source
        //{
        //    Undefined = -1,
        //    RevoAgent
        //}

        //public enum Stato
        //{
        //    Undefined = -1,
        //    Active = 1,
        //    Test = 2,
        //    Disabled = 3
        //}


        private const string _sqlElencoImmobili = "SELECT UTENTI.my_login, UTENTI.user_id, UTENTI.customer_id, ANNUNCIO.annuncio_id, ANNUNCIO.regione, ANNUNCIO.provincia, ANNUNCIO.MQ, ANNUNCIO.piano, ANNUNCIO.stato, ANNUNCIO.CAP, ANNUNCIO.COMUNE, ANNUNCIO.terrazzi, ANNUNCIO.indirizzo, ANNUNCIO.posto_auto" +
            ", ANNUNCIO.cantina, ANNUNCIO.cucina, ANNUNCIO.box, ANNUNCIO.salone, ANNUNCIO.camere_da_letto, ANNUNCIO.camerette, ANNUNCIO.bagni, ANNUNCIO.classe_energetica, ANNUNCIO.quartiere, ANNUNCIO.latitude, ANNUNCIO.longitude " +
            ", FORMAT (ANNUNCIO.date_added,\"dd-MM-yyyy\") as date_added " +
            ", FORMAT (ANNUNCIO.date_added,\"yyyyMMdd\") as date_ordered " +
            ", ANNUNCIO.tipo, ANNUNCIO.prezzo, categorie.nome AS categoria, categorie.categoria_id" +
            " FROM categorie INNER JOIN (ANNUNCIO LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id) ON categorie.categoria_id=ANNUNCIO.fk_categoria_id";
        //LA CONDIZIONE WHERE E' ESCLUSA


        public ImmobiliareManager(string connectionName)
            : base(connectionName)
        {

        }



        public List<Models.Immobile> getList()
        {
            List<Models.Immobile> risultato;
            risultato = new List<Models.Immobile>();

            mStrSQL = _sqlElencoImmobili;
            mStrSQL += " WHERE ANNUNCIO.date_deleted Is Null ";

            mDt = mFillDataTable(mStrSQL);

            foreach (DataRow row in mDt.Rows)
            {
                risultato.Add(new Models.Immobile(row, Models.Immobile.SelectFileds.Lista));
            }

            return risultato;
        }


        public List<Models.Immobile> getList(out int totalRecords, int pageSize = -1,
                                      int pageIndex = -1, string sort = "ANNUNCIO.date_added",
                                      string sortOrder = "DESC")
        {
            return getList(null, out totalRecords, pageSize, pageIndex, sort, sortOrder);
        }


        public List<Models.Immobile> getList(Models.SearchImmobili parameters, out int totalRecords, int pageSize = -1,
                                         int pageIndex = -1, string sort = "ANNUNCIO.date_added",
                                         string sortOrder = "DESC")
        {

            List<Models.Immobile> risultato;
            risultato = new List<Models.Immobile>();

            mStrSQL = _sqlElencoImmobili;
            mStrSQL += " WHERE ANNUNCIO.date_deleted Is Null ";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();


            string strWHERE = "";

            if (parameters != null)
            {

                switch (parameters.Tempo)
                {
                    case Models.SearchImmobili.EnumTempo.Oggi:
                        //strWHERE += "AND FORMAT (ANNUNCIO.date_added,\"yyyy-MM-dd\") = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                        strWHERE += String.Format(" AND (DAY({0})={1} AND  MONTH({0})={2} AND YEAR({0})={3}) ", "ANNUNCIO.date_added", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                        break;
                    case Models.SearchImmobili.EnumTempo.UltimaSettimana:
                        strWHERE += "AND ( FORMAT (ANNUNCIO.date_added,\"yyyy-MM-dd\") <= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND  FORMAT (ANNUNCIO.date_added,\"yyyy-MM-dd\") >= '" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + "')";
                        break;
                    case Models.SearchImmobili.EnumTempo.UltimoMese:
                        strWHERE += "AND ( FORMAT (ANNUNCIO.date_added,\"yyyy-MM-dd\") <= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND  FORMAT (ANNUNCIO.date_added,\"yyyy-MM-dd\") >= '" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "')";
                        break;
                }


                if (parameters.filter.regioneId != -1 && parameters.filter.regioneId != 0)
                {
                    strWHERE += " AND regione_id = " + parameters.filter.regioneId;
                }

                if (!String.IsNullOrEmpty(parameters.filter.provinciaId) && parameters.filter.provinciaId != "-1" && parameters.filter.provinciaId != "---")
                {
                    strWHERE += " AND provincia_id = @PROVINCIA_ID ";
                    mAddParameter(command, "@PROVINCIA_ID", parameters.filter.provinciaId);
                }

                if (!String.IsNullOrEmpty(parameters.filter.comuneId) && parameters.filter.comuneId != "-1" && parameters.filter.comuneId != "---")
                {
                    strWHERE += " AND comune_id =  @COMUNE_ID ";
                    mAddParameter(command, "@COMUNE_ID", parameters.filter.comuneId);
                }


                if (parameters.filter.immobile != null && parameters.filter.immobile > 0)
                {
                    strWHERE += " AND tipo = '" + parameters.filter.immobile.ToString() + "'";
                }

                if (parameters.filter.categoria != null && parameters.filter.categoria > 0)
                {
                    strWHERE += " AND categoria_id = " + ((int)parameters.filter.categoria);
                }


                if (parameters.TipoAnnuncio != null && parameters.TipoAnnuncio.Count < 2)
                {

                    if (parameters.TipoAnnuncio.Contains(Models.SearchImmobili.EnumTipoAnnuncio.Agenzia))
                    {
                        strWHERE += " AND (UTENTI.customer_id IS NOT NULL ) ";
                    }

                    if (parameters.TipoAnnuncio.Contains(Models.SearchImmobili.EnumTipoAnnuncio.Privato))
                    {
                        strWHERE += " AND (UTENTI.customer_id IS NULL ) ";
                    }

                }





                if (parameters.filter.ascensore != null && parameters.filter.ascensore != -1)
                {
                    strWHERE += " AND ascensore = " + parameters.filter.ascensore;
                }

                //16/01/2014
                if (parameters.prezzoMax != null && parameters.prezzoMax != 0)
                {
                    strWHERE += " AND prezzo <= " + parameters.prezzoMax;
                }

                //16/01/2014
                if (parameters.mqMin != null && parameters.mqMin != 0)
                {
                    strWHERE += " AND mq >= " + parameters.mqMin;
                }


            }

            if (!String.IsNullOrEmpty(strWHERE))
            {
                mStrSQL += strWHERE;
            }



            mStrSQL += " ORDER BY " + sort + " " + sortOrder;


            command.CommandText = mStrSQL;

            mDt = mFillDataTable(command);

            totalRecords = mDt.Rows.Count;


            if (pageSize > 0 && pageIndex >= 0)
            {
                // apply paging
                IEnumerable<DataRow> rows = mDt.AsEnumerable().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                foreach (DataRow row in rows)
                {
                    risultato.Add(new Models.Immobile(row, Models.Immobile.SelectFileds.Lista));
                }
            }
            else
            {
                foreach (DataRow row in mDt.Rows)
                {
                    risultato.Add(new Models.Immobile(row, Models.Immobile.SelectFileds.Lista));
                }
            }
            return risultato;
        }



        public Models.Immobile getImmobile(long id)
        {
            mStrSQL = "SELECT UTENTI.my_login AS my_login, UTENTI.user_id AS user_id, UTENTI.isModeratore AS isModeratore " +
                     ", ANNUNCIO.*  " +
                     ", categorie.nome AS categoria " +
                     ", categorie.categoria_id as categoria_id " +
                     ", UTENTI.customer_id AS customer_id " +
                     " FROM categorie INNER JOIN (ANNUNCIO LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id) ON categorie.categoria_id=ANNUNCIO.fk_categoria_id " +
                     " WHERE ( ANNUNCIO.date_deleted Is Null) And ANNUNCIO.ANNUNCIO_ID = " + id;

            mDt = mFillDataTable(mStrSQL);

            if (mDt.Rows.Count == 0)
            {
                return null;
            }

            if (mDt.Rows.Count > 1)
            {
                throw new MyManagerCSharp.MyException("id > 0");
            }

            Models.Immobile i;
            i = new Models.Immobile(mDt.Rows[0], Models.Immobile.SelectFileds.Full);

            return i;
        }

        public List<Models.Immobile> getMap()
        {
            List<Models.Immobile> risultato;
            risultato = new List<Models.Immobile>();

            mStrSQL = _sqlElencoImmobili;
            mStrSQL += " WHERE ANNUNCIO.date_deleted Is Null ";
            mStrSQL += " AND latitude > 0 AND longitude > 0";

            mDt = mFillDataTable(mStrSQL);

            foreach (DataRow row in mDt.Rows)
            {
                risultato.Add(new Models.Immobile(row, Models.Immobile.SelectFileds.Lista));
            }

            return risultato;


        }



        public void annuncioAddClick(long annuncioId)
        {
            AnnuncioManager m = new AnnuncioManager(mConnection);
            m.annuncioAddClick(annuncioId);
        }

        public void resetContatoreParziale(long annuncioId)
        {
            AnnuncioManager m = new AnnuncioManager(mConnection);
            m.resetContatoreParziale(annuncioId);
        }


        public int updateAnnuncioDescrizione(long annuncioId, string descrizione, bool test_mode)
        {
            AnnuncioManager m = new AnnuncioManager(mConnection);
            return m.updateAnnuncioDescrizione(annuncioId, descrizione, test_mode);
        }

        public int updateAnnuncioPrezzo(long annuncioId, decimal prezzo, bool test_mode)
        {
            AnnuncioManager m = new AnnuncioManager(mConnection);
            return m.updateAnnuncioPrezzo(annuncioId, prezzo, test_mode);
        }



        public System.Data.DataTable getPhoto(long annuncioId)
        {
            mStrSQL = "SELECT * FROM PHOTO WHERE  FK_EXTERNAL_ID= " + annuncioId;
            return mFillDataTable(mStrSQL);
        }



        public System.Data.DataTable getEmailUtentiInTrattativa(long annuncio_id)
        {
            //prelevo gli indizzi email di tutti gli utenti che stanno in trattativa su un annuncio
            //per inviargli un'email
            mStrSQL = "SELECT utenti.my_login, utenti.email " +
                    " FROM utenti INNER JOIN trattativa ON utenti.user_id = trattativa.fk_user_id " +
                    " WHERE trattativa.fk_annuncio_id = " + annuncio_id;
            return mFillDataTable(mStrSQL);
        }



        public int countAnnunci(long userId)
        {
            mStrSQL = "SELECT count(*) from  ANNUNCIO where  ANNUNCIO.fk_user_id = " + userId;
            return int.Parse(mExecuteScalar(mStrSQL));
        }
        public System.Collections.Hashtable countAnnunciByStato()
        {
            AnnuncioManager m = new AnnuncioManager (mConnection);
            return m.countAnnunciByStato();
        }

        public System.Collections.Hashtable countAnnunciByStato(long userId)
        {
            AnnuncioManager m = new AnnuncioManager(mConnection);
            return m.countAnnunciByStato(userId);
        }


        public System.Collections.Hashtable countTrattativeByStato()
        {
            TrattativaManager m = new TrattativaManager(mConnection);
            return m.countTrattativeByStato();
        }

        public System.Collections.Hashtable countTrattativeByStato(long userId)
        {
            TrattativaManager m = new TrattativaManager(mConnection);
            return m.countTrattativeByStato(userId);
        }


        public List<Models.Immobile> getListAnnunci(long userId)
        {



            List<Models.Immobile> risultato;
            risultato = new List<Models.Immobile>();

            //Debug
            //userId = 567809036;

            //            mStrSQL = "SELECT UTENTI.my_login, UTENTI.user_id, ANNUNCIO.annuncio_id, FORMAT(ANNUNCIO.date_added,\"dd-MM-yyyy\") AS date_added, ANNUNCIO.tipo, ANNUNCIO.prezzo,  ANNUNCIO.source_id, categorie.nome AS categoria, categorie.categoria_id " +


            //mStrSQL = "SELECT UTENTI.my_login, UTENTI.user_id, UTENTI.customer_id, ANNUNCIO.annuncio_id, ANNUNCIO.regione, ANNUNCIO.provincia, ANNUNCIO.MQ, ANNUNCIO.piano, ANNUNCIO.stato, ANNUNCIO.CAP, ANNUNCIO.COMUNE, ANNUNCIO.terrazzi, ANNUNCIO.indirizzo, ANNUNCIO.posto_auto" +
            //", ANNUNCIO.cantina, ANNUNCIO.cucina, ANNUNCIO.box, ANNUNCIO.salone, ANNUNCIO.camere_da_letto, ANNUNCIO.camerette, ANNUNCIO.bagni, ANNUNCIO.classe_energetica, ANNUNCIO.quartiere, ANNUNCIO.latitude, ANNUNCIO.longitude " +

            //", FORMAT (ANNUNCIO.date_added,\"dd-MM-yyyy\") as date_added " +
            //", FORMAT (ANNUNCIO.date_added,\"yyyyMMdd\") as date_ordered " +
            //            ", ANNUNCIO.tipo, ANNUNCIO.prezzo, categorie.nome AS categoria, categorie.categoria_id" +
            //        " FROM categorie INNER JOIN (ANNUNCIO LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id) ON categorie.categoria_id=ANNUNCIO.fk_categoria_id " 

            mStrSQL = _sqlElencoImmobili;
            mStrSQL += " WHERE ANNUNCIO.date_deleted Is Null   And ANNUNCIO.fk_user_id= " + userId;

            //    if Me.chkAnnunciEsterni.Checked {
            //    mStrSQL+= " AND  ANNUNCIO.SOURCE_ID is null "
            //}


            mStrSQL += " ORDER BY ANNUNCIO.date_added DESC";

            mDt = mFillDataTable(mStrSQL);


            Models.Immobile immobile;

            foreach (DataRow row in mDt.Rows)
            {

                immobile = new Models.Immobile(row, Models.Immobile.SelectFileds.Lista);


                risultato.Add(immobile);
            }

            return risultato;


        }


        public long insertAnnuncio(Models.Immobile immobile, long userId)
        {
            return insertAnnuncio(immobile, userId, false, 0, DateTime.MinValue);
        }

        public long insertAnnuncioInTestMode(Models.Immobile immobile, long userId)
        {
            return insertAnnuncio(immobile, userId, true, 0, DateTime.MinValue);
        }


        public long insertAnnuncio(Models.Immobile immobile, long userId, bool test_mode, AnnuncioManager.StatoAnnuncio myStato, DateTime dateAdded)
        {

            if (immobile.categoria == null || immobile.categoria == 0)
            {
                throw new MyManagerCSharp.MyException("La Categoria deve essere obbiligatoria");
            }

            string strSQLParametri;

            mStrSQL = "INSERT INTO ANNUNCIO ( FK_CATEGORIA_ID , MY_STATO";
            strSQLParametri = " VALUES ( " + (int)immobile.categoria;

            if (myStato == 0)
            {
                strSQLParametri += ", '" + AnnuncioManager.StatoAnnuncio.Pubblicato.ToString() + "' ";
            }
            else
            {
                strSQLParametri += ", '" + myStato.ToString() + "' ";
            }


            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();


            //'27/01/2012 iposto data di modifica =  data inserimento 
            DateTime dataCorrente = DateTime.Now;

            mStrSQL += ",DATE_ADDED, DATE_MODIFIED";
            strSQLParametri += ", @DATE_ADDED , @DATE_MODIFIED ";

            if (dateAdded != DateTime.MinValue)
            {
                mAddParameter(command, "@DATE_ADDED", dateAdded);
                mAddParameter(command, "@DATE_MODIFIED", dateAdded);
            }
            else
            {
                mAddParameter(command, "@DATE_ADDED", dataCorrente);
                mAddParameter(command, "@DATE_MODIFIED", dataCorrente);
            }

            if (userId != 0)
            {
                mStrSQL += ",FK_USER_ID ";
                strSQLParametri += ", @FK_USER_ID ";
                mAddParameter(command, "@FK_USER_ID", userId);
            }

            mStrSQL += ",TIPO ";
            strSQLParametri += ", @TIPO ";
            mAddParameter(command, "@TIPO", immobile.immobile.ToString());

            if (!String.IsNullOrEmpty(immobile.nota))
            {
                mStrSQL += ",DESCRIZIONE ";
                strSQLParametri += ", @DESCRIZIONE ";
                mAddParameter(command, "@DESCRIZIONE", immobile.nota);
            }

            if (immobile.MQ > 0)
            {
                mStrSQL += ",MQ ";
                strSQLParametri += ", @MQ ";
                mAddParameter(command, "@MQ", immobile.MQ);
            }

            if (immobile.piano != null && immobile.piano != int.MinValue)
            {
                mStrSQL += ",PIANO ";
                strSQLParametri += ", @PIANO ";
                mAddParameter(command, "@PIANO", immobile.piano);
            }

            if (immobile.pianiTotali != null && immobile.pianiTotali != int.MinValue)
            {
                mStrSQL += ",PIANI_TOTALI ";
                strSQLParametri += ", @PIANI_TOTALI ";
                mAddParameter(command, "@PIANI_TOTALI", immobile.pianiTotali);
            }

            if (immobile.anno != null && immobile.anno > 0)
            {
                mStrSQL += ",ANNO ";
                strSQLParametri += ", @ANNO ";
                mAddParameter(command, "@ANNO", immobile.anno);
            }

            if (immobile.cucina != null && immobile.cucina != Models.Immobile.TipoCucina.Undefined)
            {
                mStrSQL += ",CUCINA ";
                strSQLParametri += ", @CUCINA ";
                //'addParameter(command, "@CUCINA", immobile.cucina.ToString.Replace("_", " "))
                mAddParameter(command, "@CUCINA", immobile.cucina.ToString());
            }


            if (immobile.salone != null && immobile.salone != Models.Immobile.TipoSalone.Undefined)
            {
                mStrSQL += ",SALONE ";
                strSQLParametri += ", @SALONE ";
                mAddParameter(command, "@SALONE", immobile.salone.ToString());
            }


            if (immobile.statoImmobile != null && immobile.statoImmobile != Models.Immobile.TipoStatoImmobile.Undefined)
            {
                mStrSQL += ",STATO ";
                strSQLParametri += ", @STATO ";
                //'addParameter(command, "@STATO", immobile.statoImmobile.ToString.Replace("_", " "))
                mAddParameter(command, "@STATO", immobile.statoImmobile.ToString());
            }


            if (immobile.occupazione != null && immobile.occupazione != Models.Immobile.TipoOccupazione.Undefined)
            {
                mStrSQL += ",OCCUPAZIONE ";
                strSQLParametri += ", @OCCUPAZIONE ";
                //'addParameter(command, "@OCCUPAZIONE", immobile.occupazione.ToString.Replace("_", " "))
                mAddParameter(command, "@OCCUPAZIONE", immobile.occupazione.ToString());
            }


            if (immobile.box != null && immobile.box != Models.Immobile.TipoBoxAuto.Undefined)
            {
                mStrSQL += ",BOX ";
                strSQLParametri += ", @BOX ";
                mAddParameter(command, "@BOX", immobile.box.ToString());
            }

            if (immobile.postoAuto != null && immobile.postoAuto != Models.Immobile.TipoPostoAuto.Undefined)
            {
                mStrSQL += ",POSTO_AUTO ";
                strSQLParametri += ", @POSTO_AUTO ";
                //'addParameter(command, "@POSTO_AUTO", immobile.postoAuto.ToString.Replace("_", " "))
                mAddParameter(command, "@POSTO_AUTO", immobile.postoAuto.ToString());
            }


            if (immobile.prezzo > 0)
            {
                mStrSQL += ",PREZZO ";
                strSQLParametri += ", @PREZZO ";
                mAddParameter(command, "@PREZZO", immobile.prezzo);
            }

            if (!String.IsNullOrEmpty(immobile.indirizzo))
            {
                mStrSQL += ",INDIRIZZO ";
                strSQLParametri += ", @INDIRIZZO ";
                mAddParameter(command, "@INDIRIZZO", immobile.indirizzo);
            }

            if (!String.IsNullOrEmpty(immobile.regione))
            {
                mStrSQL += ",REGIONE ";
                strSQLParametri += ", @REGIONE ";
                mAddParameter(command, "@REGIONE", immobile.regione);
            }

            if (!String.IsNullOrEmpty(immobile.provincia))
            {
                mStrSQL += ",PROVINCIA ";
                strSQLParametri += ", @PROVINCIA ";
                mAddParameter(command, "@PROVINCIA", immobile.provincia);
            }

            if (!String.IsNullOrEmpty(immobile.comune))
            {
                mStrSQL += ", COMUNE ";
                strSQLParametri += ", @COMUNE ";
                mAddParameter(command, "@COMUNE", immobile.comune);
            }


            if (immobile.regioneId != -1)
            {
                mStrSQL += ",REGIONE_ID ";
                strSQLParametri += ", @REGIONE_ID ";
                mAddParameter(command, "@REGIONE_ID", immobile.regioneId);
            }

            //'if (immobile.provinciaId != -1 {
            if (!String.IsNullOrEmpty(immobile.provinciaId))
            {
                mStrSQL += ",PROVINCIA_ID ";
                strSQLParametri += ", @PROVINCIA_ID ";
                mAddParameter(command, "@PROVINCIA_ID", immobile.provinciaId);
            }

            //'if (immobile.comuneId != -1 {
            if (!String.IsNullOrEmpty(immobile.comuneId))
            {
                mStrSQL += ", COMUNE_ID ";
                strSQLParametri += ", @COMUNE_ID ";
                mAddParameter(command, "@COMUNE_ID", immobile.comuneId);
            }



            if (!String.IsNullOrEmpty(immobile.quartiere))
            {
                mStrSQL += ",QUARTIERE ";
                strSQLParametri += ", @QUARTIERE ";
                mAddParameter(command, "@QUARTIERE", immobile.quartiere);
            }

            if (!String.IsNullOrEmpty(immobile.cap))
            {
                mStrSQL += ",CAP ";
                strSQLParametri += ", @CAP ";
                mAddParameter(command, "@CAP", immobile.cap);
            }

            if (immobile.camereDaLetto != null && immobile.camereDaLetto != -1)
            {
                mStrSQL += ",CAMERE_DA_LETTO ";
                strSQLParametri += ", @CAMERE_DA_LETTO ";
                mAddParameter(command, "@CAMERE_DA_LETTO", immobile.camereDaLetto);
            }

            if (immobile.camerette != null && immobile.camerette != -1)
            {
                mStrSQL += ",CAMERETTE ";
                strSQLParametri += ", @CAMERETTE ";
                mAddParameter(command, "@CAMERETTE", immobile.camerette);
            }

            if (immobile.bagni != null && immobile.bagni != -1)
            {
                mStrSQL += ",BAGNI ";
                strSQLParametri += ", @BAGNI ";
                mAddParameter(command, "@BAGNI", immobile.bagni);
            }

            if (immobile.balconi != null && immobile.balconi != -1)
            {
                mStrSQL += ",BALCONI ";
                strSQLParametri += ", @BALCONI ";
                mAddParameter(command, "@BALCONI", immobile.balconi);
            }

            if (immobile.terrazzi != null && immobile.terrazzi != -1)
            {
                mStrSQL += ",TERRAZZI ";
                strSQLParametri += ", @TERRAZZI ";
                mAddParameter(command, "@TERRAZZI", immobile.terrazzi);
            }


            if (immobile.giardinoMq != null && immobile.giardinoMq != int.MinValue)
            {
                mStrSQL += ",GIARDINO_MQ ";
                strSQLParametri += ", @GIARDINO_MQ ";
                mAddParameter(command, "@GIARDINO_MQ", immobile.giardinoMq);
            }


            if (immobile.cantina != null && immobile.cantina != -1)
            {
                mStrSQL += ",CANTINA ";
                strSQLParametri += ", @CANTINA ";
                mAddParameter(command, "@CANTINA", immobile.cantina);
            }

            if (immobile.soffitta != null && immobile.soffitta != -1)
            {
                mStrSQL += ",SOFFITTA ";
                strSQLParametri += ", @SOFFITTA ";
                mAddParameter(command, "@SOFFITTA", immobile.soffitta);
            }


            if (immobile.climatizzato != null && immobile.climatizzato != -1)
            {
                mStrSQL += ",CLIMATIZZATO ";
                strSQLParametri += ", @CLIMATIZZATO ";
                mAddParameter(command, "@CLIMATIZZATO", immobile.climatizzato);
            }


            if (immobile.ascensore != null && immobile.ascensore != -1)
            {
                mStrSQL += ",ASCENSORE ";
                strSQLParametri += ", @ASCENSORE ";
                mAddParameter(command, "@ASCENSORE", immobile.ascensore);
            }

            if (immobile.portiere != null && immobile.portiere != -1)
            {
                mStrSQL += ",PORTIERE ";
                strSQLParametri += ", @PORTIERE ";
                mAddParameter(command, "@PORTIERE", immobile.portiere);
            }

            if (immobile.speseCondominiali != null && immobile.speseCondominiali > 0)
            {
                mStrSQL += ",SPESE_CONDOMINIALI ";
                strSQLParametri += ", @SPESE_CONDOMINIALI ";
                mAddParameter(command, "@SPESE_CONDOMINIALI", immobile.speseCondominiali);
            }

            //'REL. 1.0.0.7 del 05/01/2012
            //if ( !String.IsNullOrEmpty(immobile.classeEnergetica)) {
            if (immobile.classeEnergetica != null && immobile.classeEnergetica != Models.Immobile.TipoClasseEnergetica.Undefined)
            {
                mStrSQL += ",CLASSE_ENERGETICA ";
                strSQLParametri += ", @CLASSE_ENERGETICA ";
                mAddParameter(command, "@CLASSE_ENERGETICA", immobile.classeEnergetica.ToString());
            }



            if (immobile.riscaldamento != null && immobile.riscaldamento != Models.Immobile.TipoRiscaldamento.Undefined)
            {
                mStrSQL += ",RISCALDAMENTO ";
                strSQLParametri += ", @RISCALDAMENTO ";
                //'addParameter(command, "@RISCALDAMENTO", immobile.riscaldamento.ToString.Replace("_", " "))
                mAddParameter(command, "@RISCALDAMENTO", immobile.riscaldamento.ToString());
            }


            if (immobile.latitude != 0)
            {
                mStrSQL += ",LATITUDE ";
                strSQLParametri += "," + immobile.latitude.ToString("G18").Replace(",", ".");


                //'25/02/2011  utilizzando i parametri mi tronca i valori!!

                //'strSQLParametri+= ", @LATITUDE "
                //'addParameter(command, "@LATITUDE", immobile.latitude)
            }

            if (immobile.longitude != 0)
            {
                mStrSQL += ",LONGITUDE ";

                strSQLParametri += "," + immobile.longitude.ToString("G18").Replace(",", ".");
                //'strSQLParametri+= ", @LONGITUDE "
                //'addParameter(command, "@LONGITUDE", immobile.longitude)
            }



            //'24/01/2012
            if (immobile.sourceId != 0 && immobile.sourceId != Models.Immobile.TipoSourceId.Undefined)
            {
                mStrSQL += ",SOURCE_ID ";
                strSQLParametri += ", @SOURCE_ID ";
                mAddParameter(command, "@SOURCE_ID", (int)immobile.sourceId);
            }

            if (!String.IsNullOrEmpty(immobile.externalId))
            {
                mStrSQL += ",EXTERNAL_ID ";
                strSQLParametri += ", @EXTERNAL_ID ";
                mAddParameter(command, "@EXTERNAL_ID", immobile.externalId);
            }


            command.CommandText = mStrSQL + " ) " + strSQLParametri + " )";

            if (test_mode == true)
            {
                mTransactionBegin();
                mExecuteNoQuery(command);
                mTransactionRollback();
                return -1;
            }

            mExecuteNoQuery(command);

            return mGetIdentity();
        }



        #region "Utenti"



        public long insertUser(long userId, string nome, string cognome, string email, string mylogin, long customerId)
        {
            //'in questo caso la userId non è un contatore in quanto il valore viene gestito da UserManager
            mStrSQL = "INSERT INTO UTENTI ( USER_ID,  NOME, COGNOME, MY_LOGIN, EMAIL , CUSTOMER_ID )" +
                " VALUES ( @USER_ID , @NOME , @COGNOME , @LOGIN , @EMAIL , @CUSTOMER_ID )";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();
            command.CommandText = mStrSQL;

            mAddParameter(command, "@USER_ID", userId);
            mAddParameter(command, "@NOME", nome.Trim());
            mAddParameter(command, "@COGNOME", cognome.Trim());
            mAddParameter(command, "@LOGIN", mylogin.Trim());
            mAddParameter(command, "@EMAIL", email.Trim());

            if (customerId == -1)
            {
                mAddParameter(command, "@CUSTOMER_ID", DBNull.Value);
            }
            else
            {
                mAddParameter(command, "@CUSTOMER_ID", customerId);
            }

            mExecuteNoQuery(command);

            return mGetIdentity();
        }


        public bool deleteUser(long userId, string absoluteServerPath)
        {
            AnnuncioManager ma = new AnnuncioManager(this.mConnection);

            return ma.deleteUser(userId, absoluteServerPath);
        }


        public List<Models.MyUser> getUsers(out int totalRecords, Models.MyUser filter, int pageSize = -1, int pageIndex = -1, string sort = "MY_LOGIN", string sortOrder = "ASC")
        {
            List<Models.MyUser> risultato;
            risultato = new List<Models.MyUser>();

            mStrSQL = "SELECT *  from utenti  LEFT  join source on utenti.source_id = source.source_id  ";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();



            string strWHERE = "";
            if (filter != null)
            {
                if (filter.userId != null)
                {
                    strWHERE += " AND user_id = " + filter.userId;
                }

                if (!String.IsNullOrEmpty(filter.nome))
                {
                    strWHERE += " AND UCASE(nome) like  @NOME";
                    mAddParameter(command, "@NOME", "%" + filter.nome.ToUpper().Trim()  + "%");
                }

                if (!String.IsNullOrEmpty(filter.cognome))
                {
                    strWHERE += " AND UCASE(cognome) like  @COGNOME";
                    mAddParameter(command, "@COGNOME", "%" + filter.cognome.ToUpper().Trim() + "%");
                }

                if (!String.IsNullOrEmpty(filter.email))
                {
                    strWHERE += " AND UCASE(email) like  @EMAIL";
                    mAddParameter(command, "@EMAIL", "%" + filter.email.ToUpper().Trim() + "%");
                }

                if (!String.IsNullOrEmpty(filter.login))
                {
                    strWHERE += " AND UCASE(my_login) like  @MY_LOGIN";
                    mAddParameter(command, "@MY_LOGIN", "%" + filter.login.ToUpper().Trim() + "%");
                }

                if (!String.IsNullOrEmpty(filter.externalId))
                {
                    strWHERE += " AND UCASE(EXTERNAL_ID) like  @EXTERNAL_ID";
                    mAddParameter(command, "@EXTERNAL_ID", "%" + filter.externalId.Trim() + "%");
                }

                if (filter.sourceId != null && filter.sourceId != Models.Immobile.TipoSourceId.Undefined)
                {
                    strWHERE += " AND utenti.source_id  = " + (int) filter.sourceId;
                }

                if (filter.statoId != null && filter.statoId != Models.Immobile.StatoUtente.Undefined)
                {
                    strWHERE += " AND utenti.stato_id  = " + (int) filter.statoId;
                }

            }


            if (!String.IsNullOrEmpty(strWHERE))
            {
                mStrSQL += " WHERE (1=1) " + strWHERE;
            }


            mStrSQL += " ORDER BY " + sort + " " + sortOrder;


            command.CommandText = mStrSQL;

            mDt = mFillDataTable(command);

            totalRecords = mDt.Rows.Count;


            if (pageSize > 0 && pageIndex >= 0)
            {
                // apply paging
                IEnumerable<DataRow> rows = mDt.AsEnumerable().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                foreach (DataRow row in rows)
                {
                    risultato.Add(new Models.MyUser(row));
                }
            }
            else
            {
                foreach (DataRow row in mDt.Rows)
                {
                    risultato.Add(new Models.MyUser(row));
                }
            }

            return risultato;
        }


        public Models.MyUser getUser(long id)
        {
            mStrSQL = "SELECT * FROM UTENTI LEFT  join source on utenti.source_id = source.source_id WHERE user_id = " + id;

            mDt = mFillDataTable(mStrSQL);

            if (mDt.Rows.Count > 1)
            {
                throw new MyManagerCSharp.MyException("id > 1");
            }

            Models.MyUser u;
            u = new Models.MyUser(mDt.Rows[0]);

            return u;

        }


        public Models.MyUser getUtenteByExternalId(string externalId, Models.Immobile.TipoSourceId? sourceId)
        {
            return getUtenteByExternalId(externalId, sourceId, -1);
        }


        public Models.MyUser getUtenteByExternalId(string externalId, Models.Immobile.TipoSourceId? sourceId, long excludeUserId)
        {
            mStrSQL = " SELECT * FROM UTENTI LEFT  join source on utenti.source_id = source.source_id  " +
                  " WHERE EXTERNAL_ID = '" + externalId + "' ";

            if (sourceId != null)
            {
                mStrSQL += " AND utenti.SOURCE_ID = " + (int)sourceId;
            }
            if (excludeUserId != -1)
            {
                //escludo se stesso
                mStrSQL += " AND user_id <> " + excludeUserId;
            }



            mDt = mFillDataTable(mStrSQL);


            if (mDt.Rows.Count == 0)
            {
                return null;
            }

            if (mDt.Rows.Count > 1)
            {
                throw new MyManagerCSharp.MyException("id > 1");
            }

            Models.MyUser u;
            u = new Models.MyUser(mDt.Rows[0]);

            return u;
        }

        public bool update(Models.MyUser u)
        {

            mStrSQL = "UPDATE UTENTI SET DATE_MODIFIED = GetDate()  ";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();


            if (u.sourceId == null || u.sourceId == Models.Immobile.TipoSourceId.Undefined)
            {
                mStrSQL += " ,EXTERNAL_ID = NULL , SOURCE_ID = NULL ";
            }
            else
            {
                mStrSQL += " ,EXTERNAL_ID = @EXTERNAL_ID , SOURCE_ID =" + (int) u.sourceId;
                mAddParameter(command, "@EXTERNAL_ID", u.externalId);
            }

            if (u.statoId == Models.Immobile.StatoUtente.Undefined)
            {
                mStrSQL += ",STATO_ID = NULL , DATE_STATO = NULL ";
            }
            else
            {
                //veifico che lo stato precedente sia diverso!

                Models.Immobile.StatoUtente statoPrecedente;

                statoPrecedente = getStatoUtente((long) u.userId);

                if (statoPrecedente != u.statoId)
                {
                    mStrSQL += ",STATO_ID = " + (int)u.statoId + " , DATE_STATO = GetDate() ";
                }
            }


            if (String.IsNullOrEmpty(u.nota))
            {
                mStrSQL += ",NOTA = NULL ";
            }
            else
            {
                mStrSQL += ",NOTA = @NOTA ";
                mAddParameter(command, "@NOTA", u.nota);
            }


            mStrSQL += " WHERE USER_ID=" + u.userId;

            command.CommandText = mStrSQL;

            mExecuteNoQuery(command);

            return true;
        }


        Models.Immobile.StatoUtente getStatoUtente(long userId)
        {
            //uso temp invece di mStrSQL in quanto uso questa funzione all'inerno del manager e qundi mi va a modificare la 
            // query originale
            string temp = "SELECT STATO_ID FROM UTENTI WHERE USER_ID = " + userId;

            string stato;
            stato = mExecuteScalar(temp);

            if (String.IsNullOrEmpty(stato))
            {
                return Models.Immobile.StatoUtente.Undefined;
            }

            return  (Models.Immobile.StatoUtente)Enum.Parse(typeof(Models.Immobile.StatoUtente), stato);
        }


        #endregion

        #region "Trattative"


        public List<Models.Trattativa> getListTrattative(long userId)
        {
            List<Models.Trattativa> risultato;
            risultato = new List<Models.Trattativa>();

            //Debug
            //userId = 567809036;


            mStrSQL = "SELECT DISTINCT TRATTATIVA.trattativa_id, TRATTATIVA.stato, UTENTI.my_login, UTENTI.user_id, ANNUNCIO.annuncio_id, ANNUNCIO.date_added, ANNUNCIO.tipo, ANNUNCIO.prezzo, categorie.nome AS categoria, categorie.categoria_id " +
                " FROM ((TRATTATIVA INNER JOIN ANNUNCIO ON ANNUNCIO.annuncio_id=TRATTATIVA.fk_annuncio_id) LEFT JOIN CATEGORIE ON categorie.categoria_id=ANNUNCIO.fk_categoria_id) LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id " +
                " WHERE TRATTATIVA.fk_user_id= " + userId + "  AND  TRATTATIVA.DATE_DELETED IS NULL ";


            mDt = mFillDataTable(mStrSQL);


            Models.Trattativa trattativa;

            foreach (DataRow row in mDt.Rows)
            {

                trattativa = new Models.Trattativa(row);

                //trattativa.userId = userId;
                //trattativa.login = row["my_login"].ToString();
                //trattativa.trattativaId = long.Parse(row["trattativa_id"].ToString());
                //trattativa.annuncioId = long.Parse(row["annuncio_id"].ToString());
                //trattativa.dateAdded = (row["date_added"] is DBNull) ? DateTime.MinValue : DateTime.Parse(row["date_added"].ToString());
                //trattativa.stato = (row["stato"] is DBNull) ? Models.Trattativa.Stato.Undefined : (Models.Trattativa.Stato)Enum.Parse(typeof(Models.Trattativa.Stato), row["stato"].ToString());
                //trattativa.prezzo = (row["prezzo"] is DBNull) ? 0 : Decimal.Parse(row["prezzo"].ToString());
                //trattativa.tipo = (Immobiliare.Models.Immobile.TipoImmobile)Enum.Parse(typeof(Immobiliare.Models.Immobile.TipoImmobile), row["tipo"].ToString());
                //trattativa.categoria = (Immobiliare.Models.Immobile.Categorie)int.Parse(row["categoria_id"].ToString());
                risultato.Add(trattativa);
            }

            return risultato;
        }



        public Models.Trattativa getTrattativa(long trattativaId)
        {

            mStrSQL = "SELECT DISTINCT TRATTATIVA.trattativa_id, TRATTATIVA.stato, UTENTI.my_login, UTENTI.user_id, ANNUNCIO.annuncio_id, ANNUNCIO.date_added, ANNUNCIO.tipo, ANNUNCIO.prezzo, categorie.nome AS categoria, categorie.categoria_id " +
                " FROM ((TRATTATIVA INNER JOIN ANNUNCIO ON ANNUNCIO.annuncio_id=TRATTATIVA.fk_annuncio_id) LEFT JOIN CATEGORIE ON categorie.categoria_id=ANNUNCIO.fk_categoria_id) LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id " +
                " WHERE TRATTATIVA.trattativa_id= " + trattativaId + "  AND  TRATTATIVA.DATE_DELETED IS NULL ";


            mDt = mFillDataTable(mStrSQL);


            Models.Trattativa trattativa;

            DataRow row;
            row = mDt.Rows[0];
            trattativa = new Models.Trattativa(row);

            return trattativa;
        }





        public void setRisposteFromTrattativa(Models.Trattativa trattativa)
        {
            mStrSQL = "SELECT  UTENTI.my_login as my_login , UTENTI.isModeratore as isModeratore, UTENTI.user_id as user_id, risposta_id, fk_risposta_id ,RISPOSTA.date_added, testo , TRATTATIVA.FK_ANNUNCIO_ID as annuncio_id, FK_TRATTATIVA_ID as FK_TRATTATIVA_ID, UTENTI.customer_id " +
                    " FROM TRATTATIVA INNER JOIN (RISPOSTA LEFT  JOIN UTENTI ON RISPOSTA.FK_user_ID = UTENTI.USER_ID ) ON RISPOSTA.FK_TRATTATIVA_ID = TRATTATIVA.TRATTATIVA_ID" +
                    " WHERE FK_TRATTATIVA_ID = " + trattativa.trattativaId + " AND TRATTATIVA.FK_ANNUNCIO_ID =" + trattativa.annuncioId;

            mStrSQL += " ORDER BY RISPOSTA.date_added ASC";

            mDt = mFillDataTable(mStrSQL);

            List<Models.Risposta> risposte = new List<Models.Risposta>();

            Models.Risposta risposta;

            foreach (DataRow row in mDt.Rows)
            {
                risposta = new Models.Risposta(row, trattativa.annuncioId);

                risposte.Add(risposta);
            }

            trattativa.risposte = risposte;
        }



        public Models.Risposta getRisposta(long rispostaId)
        {
            mStrSQL = "select UTENTI.user_id, UTENTI.my_login, UTENTI.customer_id, RISPOSTA.fk_trattativa_id, RISPOSTA.testo, RISPOSTA.risposta_id, RISPOSTA.fk_risposta_id,   RISPOSTA.date_added " + " FROM RISPOSTA  LEFT JOIN UTENTI ON RISPOSTA.FK_user_ID = UTENTI.USER_ID WHERE RISPOSTA_ID =" + rispostaId;
            mDt = mFillDataTable(mStrSQL);

            return new Models.Risposta(mDt.Rows[0], -1);
        }

        public long insertTrattativa(long annuncioId, long userId)
        {
            mStrSQL = "INSERT INTO TRATTATIVA ( FK_USER_ID , FK_ANNUNCIO_ID, STATO )" + " VALUES ( @USER_ID , @ANNUNCIO_ID , @STATO )";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();
            command.CommandText = mStrSQL;

            mAddParameter(command, "@USER_ID", userId);
            mAddParameter(command, "@ANNUNCIO_ID", annuncioId);
            mAddParameter(command, "@STATO", TrattativaManager.StatoTrattativa.Attiva.ToString());

            mExecuteNoQuery(command);
            return mGetIdentity();
        }

        #endregion
    }

}
