using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Helcim.PaymentGateway.Core.Services;

namespace Helcim.PaymentGateway.Web.Services
{
    public class HelcimClient : IHelcimClient
    {
        #region Implementation of IHelcimClient

        public string MakeRequest(string endpoint, Dictionary<string, string> requestValues)
        {
            var paramCollection = new NameValueCollection();
            foreach (var keyValue in requestValues)
            {
                paramCollection.Add(keyValue.Key, keyValue.Value);
            }

            using (WebClient client = new WebClient())
            {
                byte[] responsebytes = client.UploadValues(endpoint, "POST", paramCollection);
                string responsebody = Encoding.UTF8.GetString(responsebytes);
                return responsebody;
            }
        }

        #endregion
    }
}