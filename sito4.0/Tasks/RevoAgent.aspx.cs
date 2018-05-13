using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using System.Web.UI.WebControls;
using MyManagerCSharp;

namespace MyWebApplication.Tasks
{
    public partial class RevoAgent : System.Web.UI.Page
    {


        private string _pathXmlFile = "~/Public/RevoAgent/RevoAgent.xml";
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






                _logInfo = manager._processXML(Server.MapPath(_pathXmlFile), isTestMode, forceUpdate);

                System.Threading.Thread.Sleep(5000);

                //if (manager._contaRecordConErrori > 0)
                //'{
                //'*** EMAIL ***


                MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);
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