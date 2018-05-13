using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Models
{
    public class ManageModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        //la mail la leggo dalla sessione
        //public string Email { get; set; }

        public string Nome { get; set; }
        public string Cognome { get; set; }

        public DateTime? datePreviousLogin { get; set; }

        public long customerId { get; set; }

        public string pathImageProfile { get; set; }

        public string NomeCognome
        {
            get
            {
                string temp;

                temp = Nome;

                if (String.IsNullOrEmpty(temp))
                {
                    temp = Cognome;
                }
                else
                {
                    temp += " " + Cognome;
                }

                return temp;
            }
        }

        //Credenziali per CreditoLab
        public string Login { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }


    }
}