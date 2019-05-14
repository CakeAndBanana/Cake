using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace CakeBot.Helper
{
    public class CakeDeserialize
    {
        public static dynamic Json(string url)
        {
            if (!(WebRequest.Create(url) is HttpWebRequest request)) return null;
            request.ContentType = "application/json";
            var encoding = Encoding.ASCII;
            ServicePointManager
             .ServerCertificateValidationCallback +=
             (sender, cert, chain, sslPolicyErrors) => true;

            try
            {
                using (var reader =
                    new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream() ?? throw new InvalidOperationException(),
                        encoding))
                {
                    var result = reader.ReadToEnd();
                    dynamic container = JsonConvert.DeserializeObject<dynamic>(result);
                    return container;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
