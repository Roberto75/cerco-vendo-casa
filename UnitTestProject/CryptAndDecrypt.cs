using Annunci.Immobiliare;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
    [TestClass]
    public class CryptAndDecrypt
    {
        [TestMethod]
        public void TestMethod1()
        {
            string testoOriginale = "roberto.rutigliano";

            /*** Questa è la stringa da copiare nel file di configurazione ***/
            string encryptedBase64;
            encryptedBase64 = ImmobiliareSecurityManager.Encrypt(testoOriginale);
            Debug.WriteLine(String.Format("*** Encrypted string base64: {0}", encryptedBase64));


            // Decrypt the bytes to a string.
            string roundtrip = ImmobiliareSecurityManager.Decrypt(encryptedBase64);
            Debug.WriteLine(String.Format("Decrypted from byte: {0}", roundtrip));

            Assert.AreEqual(testoOriginale, roundtrip);

        }


        [TestMethod]
        public void Encrypt()
        {
            //_encrypt(@"cerco-vendo-casa.it");

            //_encrypt("password");

            _encrypt("");
        }




        private void _encrypt(string testo)
        {
            string encryptedBase64 = ImmobiliareSecurityManager.Encrypt(testo);

            /*** Questa è la stringa da copiare nel file di configurazione ***/
            Debug.WriteLine(String.Format("Encrypt {0} => {1}", testo, encryptedBase64));
        }



        [TestMethod]
        public void TestMethod2()
        {
            string testoOriginale = "roberto.rutigliano";

            string key;
            key = "r0b3rt0000002013";
            key = "r0b3rt000wkijhttopokrpty40002013";

            string IV;
            IV = "183hdtei88i98709";
            //IV = "183hdhe87fk66655f9kf0d997kd09295";

            //stringa di lunghezza 16 = > 128
            //stringa di lunghezza 32 = > 256

            //Legal min key size = 128 
            //Legal max key size = 256 
            //Legal min block size = 128 
            //Legal max block size = 256 


            if (key.Length != 16 && key.Length != 32)
            {
                Debug.WriteLine(String.Format("Key length: {0} ", key.Length));
            }


            if (IV.Length != 16 && IV.Length != 32)
            {
                Debug.WriteLine(String.Format("IV length: {0} ", IV.Length));
            }


            byte[] keyB;
            keyB = System.Text.UTF8Encoding.UTF8.GetBytes(key);
            Debug.WriteLine(String.Format("Key: {0}  byte[{1}]  bit[{2}]", key, keyB.Length, keyB.Length * 8));


            byte[] IVB;
            IVB = System.Text.UTF8Encoding.UTF8.GetBytes(IV);
            Debug.WriteLine(String.Format("IV: {0}  byte[{1}]  bit[{2}]", IV, IVB.Length, IVB.Length * 8));


            byte[] encrypted = MyManagerCSharp.SecurityManager.AESEncryptFromString(testoOriginale, key, IV);

            /*** Questa è la stringa da copiare nel file di configurazione ***/
            string encryptedBase64;
            encryptedBase64 = Convert.ToBase64String(encrypted);
            Debug.WriteLine(String.Format("*** Encrypted string base64: {0}", encryptedBase64));


            // Decrypt the bytes to a string.
            string roundtrip = MyManagerCSharp.SecurityManager.AESDecryptFromBytes(encrypted, key, IV);
            Debug.WriteLine(String.Format("Decrypted from byte: {0}", roundtrip));

            roundtrip = MyManagerCSharp.SecurityManager.AESDecryptFromBase64String(encryptedBase64, key, IV);
            Debug.WriteLine(String.Format("Decrypted from base64String: {0}", roundtrip));

        }
    
}
}
