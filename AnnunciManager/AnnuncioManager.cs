using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci
{

    public class AnnuncioManager : MyManagerCSharp.ManagerDB
    {

        public enum StatoAnnuncio
        {
            Undefined = -1,
            Pubblicato = 1,
            Oggetto_non_piu_disponibile,
            Concluso_con_successo,
            Altro,
            OffLine,
            Da_cancellare
        }


       public AnnuncioManager(string connectionName)
            : base(connectionName)
        {

        }

        public AnnuncioManager(System.Data.Common.DbConnection connection)
            : base(connection)
        {

        }


        public bool deleteUser(long userId, string absoluteServerPath)
        {
            deleteAnnunciByUserId(userId, absoluteServerPath);

            //potrebbe aver scritto delle treatta senza aver creato annunci!
            deleteTrattativeByUserId(userId);


            mStrSQL = "DELETE * FROM UTENTI WHERE USER_ID = " + userId;
            return mExecuteNoQuery(mStrSQL) > 0;
        }



        public bool deleteTrattativeByUserId(long userId)
        {
            mStrSQL = "SELECT TRATTATIVA_ID FROM TRATTATIVA WHERE FK_USER_ID = " + userId;

            mDt = mFillDataTable(mStrSQL);

            foreach (System.Data.DataRow row in mDt.Rows)
            {
                deleteTrattativa(long.Parse(row["TRATTATIVA_ID"].ToString()));
            }


            return true;
        }



        public bool deleteTrattativa(long trattativaId)
        {
            mStrSQL = "UPDATE RISPOSTA SET FK_RISPOSTA_ID = NULL WHERE FK_TRATTATIVA_ID=" + trattativaId;
            mExecuteNoQuery(mStrSQL);

            mStrSQL = "DELETE * FROM RISPOSTA  WHERE FK_TRATTATIVA_ID = " + trattativaId;
            mExecuteNoQuery(mStrSQL);

            mStrSQL = "DELETE * FROM TRATTATIVA  WHERE TRATTATIVA_ID = " + trattativaId;
            mExecuteNoQuery(mStrSQL);

            return true;
        }

        public bool deleteAnnunciByUserId(long userId, string absoluteServerPath)
        {
            mStrSQL = "SELECT ANNUNCIO_ID FROM ANNUNCIO WHERE FK_USER_ID = " + userId;

            mDt = mFillDataTable(mStrSQL);

            foreach (System.Data.DataRow row in mDt.Rows)
            {
                deleteAnnuncio(long.Parse(row["ANNUNCIO_ID"].ToString()), absoluteServerPath);
            }


            return true;
        }



        public bool deleteAnnuncio(long annuncioId, string absoluteServerPath)
        {
            //Cancellazione fisica
            mStrSQL = "SELECT TRATTATIVA_ID FROM TRATTATIVA WHERE FK_ANNUNCIO_ID=" + annuncioId;

            System.Data.DataTable trattative;

            trattative = mFillDataTable(mStrSQL);

            foreach (System.Data.DataRow row in trattative.Rows)
            {
                mStrSQL = "UPDATE RISPOSTA SET FK_RISPOSTA_ID = NULL WHERE FK_TRATTATIVA_ID=" + row["trattativa_id"];
                mExecuteNoQuery(mStrSQL);

                mStrSQL = "DELETE * FROM RISPOSTA WHERE FK_TRATTATIVA_ID=" + row["trattativa_id"];
                mExecuteNoQuery(mStrSQL);

            }



            mStrSQL = "DELETE * FROM TRATTATIVA WHERE FK_ANNUNCIO_ID=" + annuncioId;
            mExecuteNoQuery(mStrSQL);


            if (!String.IsNullOrEmpty(absoluteServerPath))
            {

                PhotoManager photo = new PhotoManager(this.mConnection);


                mStrSQL = "SELECT PHOTO_ID FROM PHOTO WHERE FK_EXTERNAL_ID=" + annuncioId;
                System.Data.DataTable dt;
                dt = mFillDataTable(mStrSQL);

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    photo.deletePhoto(long.Parse(row["PHOTO_ID"].ToString()), absoluteServerPath);
                }


                //cancello la directory!
                string folder;
                folder = System.Configuration.ConfigurationManager.AppSettings["mercatino.images.folder"];
                folder = absoluteServerPath + folder.Replace("~", "") + annuncioId + "/";
                if (System.IO.Directory.Exists(folder)) { }
                try
                {
                    System.IO.Directory.Delete(folder, true);
                }
                catch (Exception e)
                {
                    //'lo ignoro
                }

            }


            mStrSQL = "DELETE * FROM PHOTO WHERE FK_EXTERNAL_ID=" + annuncioId;
            mExecuteNoQuery(mStrSQL);

            mStrSQL = "DELETE * FROM ANNUNCIO WHERE ANNUNCIO_ID=" + annuncioId;
            mExecuteNoQuery(mStrSQL);
            return true;
        }



        public bool deleteAnnuncioLogic(long annuncioId, StatoAnnuncio causale, string absoluteServerPath)
        {
            {
                mStrSQL = "UPDATE ANNUNCIO SET DATE_DELETED = NOW() , MY_STATO = '" + causale.ToString() + "' WHERE ANNUNCIO_ID = " + annuncioId;
                mExecuteNoQuery(mStrSQL);

                //'Dim statoTrattativa As MyManager.MercatinoManager.StatoTrattativa

                //'If causale = StatoAnnuncio.ConclusoConSuccesso Then
                //'StatoTrattativa = MercatinoManager.StatoTrattativa.AnnuncioRimosso
                //'End If

                //'tutte le trattative collegate all'annuncio vengono notificate 
                mStrSQL = "UPDATE TRATTATIVA SET STATO = '" + TrattativaManager.StatoTrattativa.AnnuncioRimosso.ToString() + "' " +
                    " WHERE FK_ANNUNCIO_ID = " + annuncioId;
                mExecuteNoQuery(mStrSQL);


                if (!String.IsNullOrEmpty(absoluteServerPath))
                {

                    PhotoManager photo = new PhotoManager(mConnection);

                    mStrSQL = "SELECT PHOTO_ID FROM PHOTO WHERE FK_EXTERNAL_ID=" + annuncioId;
                    mDt = mFillDataTable(mStrSQL);


                    foreach (System.Data.DataRow row in mDt.Rows)
                    {
                        photo.deletePhoto(long.Parse(row["PHOTO_ID"].ToString()), absoluteServerPath);
                    }

                    //'cancello la directory!
                    string folder;
                    folder = System.Configuration.ConfigurationManager.AppSettings["mercatino.images.folder"];
                    folder = absoluteServerPath + folder.Replace("~/", "") + annuncioId + System.IO.Path.DirectorySeparatorChar;
                    if (System.IO.Directory.Exists(folder))
                    {
                        try
                        {
                            System.IO.Directory.Delete(folder, true);
                        }
                        catch (Exception ex)
                        {
                            //'lo ignoro
                        }
                    }

                }

                return true;
            }
        }



        public long getNumeroRisposteOfAnnuncio(long annuncioId, long userId)
        {
            //'nel conteggio delle risposte escludo le risposte scritte da se stesso
            mStrSQL = "SELECT count(*) as NUMERO_RISPOSTE " +
                        "FROM RISPOSTA  INNER JOIN  TRATTATIVA ON RISPOSTA.FK_TRATTATIVA_ID = TRATTATIVA.TRATTATIVA_ID" +
                        " WHERE FK_ANNUNCIO_ID =" + annuncioId + " AND RISPOSTA.FK_USER_ID <> " + userId;
            return long.Parse(mExecuteScalar(mStrSQL));
        }



        public int updateAnnuncioDescrizione(long annuncioId, string descrizione, bool test_mode)
        {
            mStrSQL = "UPDATE ANNUNCIO SET DATE_MODIFIED = GetDate()  ";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();

            if (!String.IsNullOrEmpty(descrizione))
            {
                mStrSQL += " ,DESCRIZIONE = @DESCRIZIONE ";
                mAddParameter(command, "@DESCRIZIONE", descrizione);
            }

            mStrSQL += " WHERE ANNUNCIO_ID=" + annuncioId;

            command.CommandText = mStrSQL;


            if (test_mode)
            {
                mTransactionBegin();
                mExecuteNoQuery(command);
                mTransactionRollback();
                return -1;
            }

            return mExecuteNoQuery(command);
        }

        public int updateAnnuncioPrezzo(long annuncioId, decimal prezzo, bool test_mode)
        {
            mStrSQL = "UPDATE ANNUNCIO SET DATE_MODIFIED = GetDate()  ";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();

            if (prezzo != Decimal.MinValue)
            {
                mStrSQL += " ,PREZZO = @PREZZO ";
                mAddParameter(command, "@PREZZO", prezzo);
            }

            mStrSQL += " WHERE ANNUNCIO_ID=" + annuncioId;

            command.CommandText = mStrSQL;


            if (test_mode)
            {
                mTransactionBegin();
                mExecuteNoQuery(command);
                mTransactionRollback();
                return -1;
            }

            return mExecuteNoQuery(command);
        }




        public void annuncioAddClick(long annuncioId)
        {
            mStrSQL = "UPDATE ANNUNCIO SET DATE_LAST_CLICK = NOW , COUNT_CLICK = COUNT_CLICK +1 , COUNT_CLICK_PARZIALE = COUNT_CLICK_PARZIALE + 1" +
                " WHERE ANNUNCIO_ID=" + annuncioId;
            mExecuteNoQuery(mStrSQL);
        }


        public void resetContatoreParziale(long annuncioId)
        {
            mStrSQL = "UPDATE ANNUNCIO SET date_start_click_parziale = NOW , COUNT_CLICK_PARZIALE = 0 " +
                                           " WHERE ANNUNCIO_ID=" + annuncioId;
            mExecuteNoQuery(mStrSQL);
        }



        public long countAnnunciByStato(StatoAnnuncio stato)
        {
            return countAnnunciByStato(stato,-1);
        }
        public long countAnnunciByStato(StatoAnnuncio stato, long userId)
        {
            mStrSQL = "SELECT COUNT(*) FROM ANNUNCIO WHERE MY_STATO = '" + stato.ToString() + "'";

            if (userId != -1)
            {
                mStrSQL += " AND fk_user_id = " + userId;
            }
            return long.Parse (mExecuteScalar(mStrSQL));
        }

        public System.Collections.Hashtable countAnnunciByStato()
        {
            return countAnnunciByStato(-1);
        }

        public System.Collections.Hashtable countAnnunciByStato(long userId)
        {
            System.Collections.Hashtable risultato = new System.Collections.Hashtable();

            foreach (var value in Enum.GetValues(typeof(StatoAnnuncio)))
            {
                risultato .Add ((StatoAnnuncio ) value , countAnnunciByStato((StatoAnnuncio ) value , userId));
            }

            return risultato ;
        }


    }
}

