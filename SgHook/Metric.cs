using System.IO;
using System.Net;
using System.Text;

namespace SgHook
{
    public class Metric
    {
        public static bool getTitle()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://metric-mai2.kiwi.cat/simplemetric");
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = "CNHook";
            request.Timeout = 1000;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    string responseString = streamReader.ReadToEnd();
                    MelonLoader.MelonLogger.Msg(responseString);
                    if(responseString.Contains("sorry"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return true;
            }
        }
    }
}
