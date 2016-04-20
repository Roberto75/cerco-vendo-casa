using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MyManagerCSharp;

namespace MyWebApplication.Tasks
{
    public partial class Test : System.Web.UI.Page
    {

        private string _pathLogFileException = "~/Public/RevoAgent/TestException.log";
        private string _pathLogFile = "~/Public/RevoAgent/Test.log";
        private string _pathXmlFile = "~/Public/RevoAgent/Test.xml";

        protected void Page_Load(object sender, EventArgs e)
        {
                executeTask();

        }


        private bool executeTask()
        {
            bool isTestMode;
            isTestMode = true;

            bool forceUpdate;
            forceUpdate = false;

            bool executeDownload;
            executeDownload = true;

            string logInfo = "";

            try
            {

                //*** DOWNLOAD XML ***

                ImmobiliareVb.RevoAgent manager = new ImmobiliareVb.RevoAgent(Server.MapPath(_pathLogFile));

                if (executeDownload)
                {
                    if (!manager._downloadFileXML(Server.MapPath(_pathXmlFile)))
                    {
                        logInfo = "FAILED download file XML";
                        MyManagerCSharp.MailManager.send(new MyManagerCSharp.MyException(logInfo));
                        return false;
                    }

                    if (System.IO.File.Exists(Server.MapPath(_pathXmlFile)))
                    {
                        System.IO.File.Delete(Server.MapPath(_pathXmlFile));
                    }

                    //'26/04/2013 OutOfMemoryException
                    //'IO.File.WriteAllText(Server.MapPath(_pathXmlFile), manager._documentXML.InnerXml)
                    //manager._documentXML.Save(Server.MapPath(_pathXmlFile));
                }

                System.Threading.Thread.Sleep(5000);

                //logInfo = manager._processXML(Server.MapPath(_pathXmlFile), isTestMode, forceUpdate);

                System.Threading.Thread.Sleep(5000);

                if (manager._contaRecordConErrori > 0)
                {
                    //'*** EMAIL ***


                    MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);
                    if (isTestMode)
                    {
                        mail.Subject = "Test task import TEST";
                    }
                    else
                    {
                        mail.Subject = "Task import TEST *** ERROR ***";
                    }

                    mail.Body = logInfo.Replace(Environment.NewLine, "<br />");
                    mail.To(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);
                    //'MY-DEBUGG
                    mail.send();
                }
            }
            catch (Exception ex)
            {
                logInfo += logInfo + ex.Message + Environment.NewLine;
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