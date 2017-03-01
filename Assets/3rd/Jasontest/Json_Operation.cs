using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MyWard.tools
{
    class Json_Operation
    {
        public static object Read_Json(string result, object o)
        {
            try
            {
                if (result == null)
                    return null;

                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(result);

                o = serializer.Deserialize(new JsonTextReader(sr), o.GetType());
            }
            catch (Exception exc)
            {
                //LogHelper.loghelper.Instance.WriteLog(exc.GetType(), exc.ToString());
            }

            return o;
        }

        //public static object Read_jsonArrat(string result)
        //{
        //    try
        //    {
        //        var statusReportInfo = JsonConvert.DeserializeObject<List<StatusReportInfo>>(result);
        //        return statusReportInfo;
        //    }
        //    catch (Exception exc)
        //    {
        //        LogHelper.loghelper.Instance.WriteLog(exc.GetType(), exc.ToString());
        //        return null;
        //    }   
        //}

        public static string Write_Json(object o)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringWriter sw = new StringWriter();
            try
            {
                serializer.Serialize(new JsonTextWriter(sw), o);
            }
            catch (Exception exc)
            {
               // LogHelper.loghelper.Instance.WriteLog(exc.GetType(), exc.ToString());
            }
            return sw.GetStringBuilder().ToString();
        }
        public static string PostJsonData(string url, string data)
        {
            HttpWebRequest webRequest1 = null;
            try
            {
                webRequest1 = (HttpWebRequest) FileWebRequest.Create(url);
                webRequest1.Proxy = null;
                webRequest1.Method = "POST";
                webRequest1.ContentType = "Application/json";

                byte[] bs = Encoding.UTF8.GetBytes(data);

                using (Stream reqStream = webRequest1.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }
                string responseString = null;
                using (HttpWebResponse response = webRequest1.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                    reader.Close();
                }
                return responseString;
            }
            catch (WebException webException)
            {
                Console.WriteLine(data);
               // LogHelper.loghelper.Instance.WriteLog(webException.GetType(),
                 //   webException.ToString() + ":" + url + ":" + data);
                return null;
            }
            catch (Exception exc)
            {
               // LogHelper.loghelper.Instance.WriteLog(exc.GetType(), exc.ToString() + ":" + url + ":" + data);
                return null;
            }
            finally
            {
                if (webRequest1 != null)
                {
                    webRequest1.Abort();
                }
            }
        }
        public static string GetJsonData(string url, string data)
        {
            try
            {
                string outUrl = url + data;

                HttpWebRequest webRequest1 = (HttpWebRequest)FileWebRequest.Create(outUrl);
                webRequest1.Method = "GET";

                HttpWebResponse response2 = (HttpWebResponse)webRequest1.GetResponse();
                StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                string responseString = sr2.ReadToEnd();

                return responseString;
            }
            catch (Exception exc)
            {
              //  LogHelper.loghelper.Instance.WriteLog(exc.GetType(), exc.ToString());
                return null;
            }
        }
    }
}
