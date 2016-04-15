using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci
{
    public class ImmobiliareMailMessageManager: MyManagerCSharp.MailManager
    {
        public enum Lingua
        {
            IT = 0,
            EN = 1
        }

        private string _http;
        private string _applicationName;


        public ImmobiliareMailMessageManager(string applicationName, string http)
        {
            _applicationName = applicationName;
            _http = http;
        }


        public string getFirma(Lingua lingua)
        {
            string risulato = "";

            switch (lingua)
            {
                case Lingua.IT:
                    risulato = "<br />Cordiali saluti dallo staff di " + _applicationName + "." + Environment.NewLine +
                      "<br><br><br> " + _applicationName + Environment.NewLine +
                      "<br><a href=\"" + _http + "\">" + _http + "</a>" + Environment.NewLine;
                    break;
            }

            return risulato;

        }



        public string getBodyImmobiliareModificaTestoAnnuncio(long annuncio_id, string titoloAnnuncio)
        {
            //una volta inserito un post invio un'email a chi ha aperto il Thread

            string temp = "";
            temp = "<h1>" + _applicationName + "</h1>" +
            " <p>Gentile utente,  " +
            "<br /> Il proprietartio dell'annuncio \"{0}\" ha modificato il testo della descrizione." +
            "<br /> " +
            "<br /> Clicca <a href=\"" + System.Configuration.ConfigurationManager.AppSettings["application.url"] + "Immobiliare/Details/{1}\">qui</a> per visualizzare le modiche apportate all'annuncio .<br /><br />";

            temp = String.Format(temp, titoloAnnuncio, annuncio_id);

            temp += getFirma(Lingua.IT);

            return temp;
        }



        public string getBodyImmobiliareAggiornamentoPrezzoAnnuncio(long annuncio_id, string titoloAnnuncio, decimal vecchioPrezzo, decimal nuovoPrezzo)
        {
            //una volta inserito un post invio un'email a chi ha aperto il Thread

            string temp = "";
            temp = "<h1>" + _applicationName + "</h1>" +
            " <p>Gentile utente,  " +
            "<br /> Il proprietartio dell'annuncio \"{0}\" ha modificato il prezzo dell'annuncio." +
            "<br /> " +
            "<p> " + String.Format("Vecchio prezzo: € {0:N2}", vecchioPrezzo) + "</p>" +
            "<p> " + String.Format("Nuovo prezzo: € {0:N2}", nuovoPrezzo) + "</p>" +
            "<br /> " +
            "<br /> Clicca <a href=\"" + System.Configuration.ConfigurationManager.AppSettings["application.url"] + "Immobiliare/Details/{1}\">qui</a> per visualizzare le modiche apportate all'annuncio .<br /><br />";

            temp = String.Format(temp, titoloAnnuncio, annuncio_id);

            temp += getFirma(Lingua.IT);

            return temp;
        }




        public string getBodyImmobiliareCancellaAnnuncio(string titoloAnnuncio)
        {
            string temp = "";
            temp = "<h1> " + _applicationName + "</h1>" +
            " <p>Gentile utente,  </p>" +
            "<br />ti segnaliamo la cancellazione dell'annuncio per: {0} " +
            "<br />" +
            "<p>Di conseguenza la compravendita è stata interrotta. </p> " +
            "<br /> Clicca <a href=\"" + System.Configuration.ConfigurationManager.AppSettings["application.url"] + "immobiliare/MyAnnunci\">qui</a> per eliminare la compravendita  dalle tue trattative.<br /><br />";

            temp = String.Format(temp, titoloAnnuncio);

            temp += getFirma(Lingua.IT);

            return temp;
        }




  
    
    }
}
