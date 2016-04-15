using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci
{
    public class TrattativaManager : MyManagerCSharp.ManagerDB
    {

        public enum StatoTrattativa
        {
            Attiva = 1,
            NonPiuDiInteresse,
            AnnuncioRimosso,
            TerminataConSuccesso,
            TerminataSenzaSuccesso,
            TerminataConFrode,
            Altro
        }
        public TrattativaManager(string connectionName)
            : base(connectionName)
        {

        }
        public TrattativaManager(System.Data.Common.DbConnection connection)
            : base(connection)
        {

        }


        public long countTrattativeByStato(StatoTrattativa stato)
        {
            return countTrattativeByStato(stato, -1);
        }


        public long countTrattativeByStato(StatoTrattativa stato, long userId)
        {
            mStrSQL = "SELECT COUNT(*) FROM TRATTATIVA WHERE STATO = '" + stato.ToString() + "'";

            if (userId != -1)
            {
                mStrSQL += " AND fk_user_id = " + userId;
            }
            return long.Parse(mExecuteScalar(mStrSQL));
        }

        public System.Collections.Hashtable countTrattativeByStato()
        {
            return countTrattativeByStato(-1);
        }

        public System.Collections.Hashtable countTrattativeByStato(long userId)
        {
            System.Collections.Hashtable risultato = new System.Collections.Hashtable();

            foreach (var value in Enum.GetValues(typeof(StatoTrattativa)))
            {
                risultato.Add((StatoTrattativa)value, countTrattativeByStato((StatoTrattativa)value, userId));
            }

            return risultato;
        }




        

        //public System.Collections.Hashtable countTrattativeByStato()
        //{
        //    return countTrattativeByStato(-1);
        //}

        //public System.Collections.Hashtable countTrattativeByStato(long userId)
        //{
        //    System.Collections.Hashtable risultato = new System.Collections.Hashtable();

        //    foreach (var value in Enum.GetValues(typeof(StatoTrattativa)))
        //    {
        //        risultato.Add((StatoTrattativa)value, countTrattativeByStato((StatoTrattativa)value, userId));
        //    }

        //    return risultato;
        //}




        public long rispondi(long trattativaId, long userId, string testo)
        {
            //una nuova risposta per l'annuncio da parte di un utente
            mStrSQL = "INSERT INTO RISPOSTA ( FK_USER_ID, FK_TRATTATIVA_ID, TESTO , FK_RISPOSTA_ID , OWNER) " +
                        " VALUES ( @FK_USER_ID ,  @FK_TRATTATIVA_ID , @TESTO , NULL, " + userId + " ) ";

            //'30/01/2011 VER. 1.0.0.5
            //'l'inserimento di una nuova risposta comporta la notifica del messaggio 


            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();
            mAddParameter(command, "@FK_USER_ID", userId);
            mAddParameter(command, "@FK_TRATTATIVA_ID", trattativaId);
            mAddParameter(command, "@TESTO", testo);

            command.CommandText = mStrSQL;

            mExecuteNoQuery(command);
            //Return Me.mGetIdentity
            return 1;
        }

        public long rispondi(long trattativaId, long userId, string testo, long risposta_id)
        {
            //'una nuova risposta per l'annuncio da parte di un utente
            mStrSQL = "SELECT OWNER FROM RISPOSTA WHERE RISPOSTA_ID  = " + risposta_id;

            long owner;
            owner = long.Parse(mExecuteScalar(mStrSQL));

            mStrSQL = "INSERT INTO RISPOSTA ( FK_USER_ID, FK_TRATTATIVA_ID, TESTO , FK_RISPOSTA_ID , OWNER) " +
                        " VALUES ( @FK_USER_ID ,  @FK_TRATTATIVA_ID , @TESTO , " + risposta_id + ", " + owner + " ) ";


            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();
            mAddParameter(command, "@FK_USER_ID", userId);
            mAddParameter(command, "@FK_TRATTATIVA_ID", trattativaId);
            mAddParameter(command, "@TESTO", testo);

            command.CommandText = mStrSQL;

            mExecuteNoQuery(command);

            //Return Me.mGetIdentity
            return 1;
        }









    }

}