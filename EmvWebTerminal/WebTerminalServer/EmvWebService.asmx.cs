using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;

namespace WebTerminalServer
{
    public enum FuncNumber { SelLang };

    /// <summary>
    /// Summary description for EmvWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class EmvWebService : System.Web.Services.WebService
    {
        EmvKernelSession EmvKerSess;

        [WebMethod(EnableSession = true)]
        public byte EmvRun()
        {
            if (Session["EmvKernel"] == null)
            {
                EmvKerSess = new EmvKernelSession();
                Session["EmvKernel"] = EmvKerSess;
            }
            else
            {
                EmvKerSess = (EmvKernelSession)Session["EmvKernel"];
            }

            EmvKerSess.mreWait = new ManualResetEvent(false); 
            EmvKerSess.mre = new ManualResetEvent(false);
            EmvKerSess.Run();
            EmvKerSess.mre.WaitOne();
            //selLang
            return (byte)EmvKerSess.FuncNum;
        }

        [WebMethod(EnableSession = true)]
        public byte SelLang(string lang)
        {
            byte res = 0xFF;//ERROR
            if (Session["EmvKernel"] != null)
            {
                EmvKerSess = (EmvKernelSession)Session["EmvKernel"];
                EmvKerSess.Language = lang;

                EmvKerSess.mre = new ManualResetEvent(false);
                EmvKerSess.mreWait.Set();
                EmvKerSess.mre.WaitOne();
                res = (byte)EmvKerSess.FuncNum;
            }
            return res;
        }

        [WebMethod(EnableSession = true)]
        public string CheckData()
        {
            return "data";
        }

        
    }
}
