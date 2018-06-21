using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace GetValue.Code
{
    public class ExternalApi
    {
        public int GetValue(int currentValue)
        {
            var httpWReq = (HttpWebRequest)WebRequest.Create("http://localhost:47983/api/values");
            httpWReq.Method = "GET";
            var x = (HttpWebResponse)httpWReq.GetResponse();
            string strResult = new StreamReader(x.GetResponseStream()).ReadToEnd();
            strResult = strResult.Replace("\"", "");

            int res = 0;
            if (int.TryParse(strResult, out res))
                currentValue *= res;

            return currentValue;
        }
    }
}