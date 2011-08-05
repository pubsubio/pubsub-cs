using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace PubsubIO.Client
{
    public class HttpUtil
    {
        public static string PostObjectAsJson(string url, object obj)
        {
            var javascriptSerializer = new JavaScriptSerializer();

            //Issue: \n should not be required in accordance with HTTP, however pubsub-hub requires this in order to support backwards compatibility with ancient browsers.
            string json = javascriptSerializer.Serialize(obj) + "\n";
            byte[] postData = Encoding.ASCII.GetBytes(json);

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentLength = postData.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowWriteStreamBuffering = false;
            request.ServicePoint.Expect100Continue = false;

            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postData, 0, postData.Length);

                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (var streamReader = new StreamReader(responseStream))
                            {
                                string result = streamReader.ReadToEnd();
                                return result;
                            }
                        }
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (var streamReader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }

                throw e;
            }
        }
    }
}