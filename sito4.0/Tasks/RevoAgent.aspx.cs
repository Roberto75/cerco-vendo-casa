using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using System.Web.UI.WebControls;
using MyManagerCSharp;
using Annunci;
using System.Diagnostics;

namespace MyWebApplication.Tasks
{
    public partial class RevoAgent : System.Web.UI.Page
    {


        private string _pathXmlFile = "~/Public/RevoAgent/RevoAgent.xml";
        //private string _pathXmlFile = "~/Public/RevoAgent/Test01.xml";

        private string _pathXmlFileMD5 = "~/Public/RevoAgent/RevoAgent.md5";

        private string _pathCategorieXML = "~/Public/RevoAgent/categorie.xml";
        private string _pathLogFile = "~/Public/RevoAgent/RevoAgent.log";
        private string _pathLogFileException = "~/Public/RevoAgent/RevoAgentException.log";

        private string _pathStatusFile = "~/public/RevoAgent/RevoAgent.status";
        private string _logInfo = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            // valore di default 110
            int timeOut = Server.ScriptTimeout;

            //Rutigliano 13/01/2014
            //non posso usare il file di log altrimenti ho un problema: is being used by another process
            //MyLog._init(Server.MapPath(_pathLogFile));
            //MyLog.Info("Inizio executeTask()");
            //MyLog.Info("Server.ScriptTimeout: " + timeOut);

            // 1 ora
            Server.ScriptTimeout = 3600;

            try
            {
                executeTask();
            }
            finally
            {
                //ripristino il valore iniziale
                Server.ScriptTimeout = timeOut;
            }
        }


        private bool executeTask()
        {

            bool isTestMode;
            isTestMode = false;

            bool forceUpdate;
            forceUpdate = false;

            bool executeDownload;
            executeDownload = true;
            //executeDownload = false;


            try
            {

                //*** DOWNLOAD XML ***

                ImmobiliareVb.RevoAgent manager = new ImmobiliareVb.RevoAgent(Server.MapPath(_pathLogFile));
                manager._pathCategorieXML = Server.MapPath(_pathCategorieXML);

                //'manager._pathLogFile = Server.MapPath(_pathLogFile)
                manager._pathStatusFile = Server.MapPath(_pathStatusFile);

                if (executeDownload)
                {
                    if (!manager._downloadFileXML(Server.MapPath(_pathXmlFile)))
                    {
                        _logInfo = "FAILED download file XML";
                        MyManagerCSharp.MailManager.send(new MyManagerCSharp.MyException(_logInfo));
                        return false;
                    }

                    //if (System.IO.File.Exists(Server.MapPath(_pathXmlFile)))
                    //{
                    //    System.IO.File.Delete(Server.MapPath(_pathXmlFile));
                    //}

                    //'26/04/2013 OutOfMemoryException
                    //'IO.File.WriteAllText(Server.MapPath(_pathXmlFile), manager._documentXML.InnerXml)
                    //manager._documentXML.Save(Server.MapPath(_pathXmlFile));
                }

                System.Threading.Thread.Sleep(5000);


                //22/09/2018 calcolo MD5 del file per verifcare se ci sono modifiche rispetto al caricamento precedente
                System.IO.FileInfo fi = new System.IO.FileInfo(Server.MapPath(_pathXmlFile));
                string md5 = SecurityManager.getMD5Hash(fi);
                Debug.WriteLine("md5: " + md5);

                System.IO.FileInfo fiMD5 = new System.IO.FileInfo(Server.MapPath(_pathXmlFileMD5));
                string md5Prcedente = "";
                if (fiMD5.Exists)
                {
                    md5Prcedente = FileManager.readTextFile(fiMD5);
                }


                if (md5Prcedente == md5)
                {
                    _logInfo = "I file XML ha lo stesso MD5 del caricamento precedente: " + md5;
                    _logInfo += "ImmobiliareVb ver." + typeof(ImmobiliareVb.RevoAgent).Assembly.GetName().Version.ToString(); 
                    MailManager.send(new MyManagerCSharp.MyException(_logInfo));
                    return false;
                }

                //sostituisco il contenuto del file con il nuovo MD5
                FileManager.writeTextFile(fiMD5, md5);
                

                _logInfo = manager._processXML(Server.MapPath(_pathXmlFile), isTestMode, forceUpdate);

                System.Threading.Thread.Sleep(5000);

                //if (manager._contaRecordConErrori > 0)
                //'{
                //'*** EMAIL ***


                ImmobiliareMailMessageManager mail = new ImmobiliareMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);
                if (isTestMode)
                {
                    mail.Subject = "Test task import RevoAgent";
                }
                else
                {
                    mail.Subject = "Task import RevoAgent *** ERROR ***";
                }

                mail.Body = _logInfo.Replace(Environment.NewLine, "<br />");
                mail.To(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);
                //'MY-DEBUGG
                mail.send();
                //}
            }
            catch (Exception ex)
            {
                _logInfo += _logInfo + ex.Message + Environment.NewLine;
                MyManagerCSharp.MailManager.send(ex);

                string msg;
                msg = ex.Message;

                if (ex.InnerException != null)
                {
                    msg += " - " + ex.InnerException.Message;
                }
                System.IO.File.WriteAllText(Server.MapPath(_pathLogFileException), msg);
            }

            return true;
        }

    }
}