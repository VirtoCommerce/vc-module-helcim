using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace Helcim.PaymentGateway.Web.Services
{
    public class HelcimService : IHelcimService
    {
        private readonly string _apiEndpoint;
        private readonly HttpClient _httpClient;

        #region Implementation of IHelcimService

        public HelcimService(string apiEndpoint, HttpClient client)
        {
            _apiEndpoint = apiEndpoint;
            _httpClient = client;
        }

        public async Task<HelcimTransactionResponse> GetTransaction(HelcimRequest request)
        {
            var action = "transactionView";
            var values = ToRequestDictionary(request, action);

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync(_apiEndpoint, content);

            var responseString = await response.Content.ReadAsStringAsync();
            var transactionResponse = responseString.DeserializeXml<HelcimTransactionResponse>();

            var result = new HelcimTransactionResponse();
            return result;
        }

        public async Task<HelcimPaymentResponse> CapturePayment(HelcimRequest request)
        {
            var action = "capture";
            var values = ToRequestDictionary(request, action);

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync(_apiEndpoint, content);

            var responseString = await response.Content.ReadAsStringAsync();

            var result = new HelcimPaymentResponse();
            return result;
        }

        private static Dictionary<string, string> ToRequestDictionary(HelcimRequest request, string action)
        {
            var values = new Dictionary<string, string>
            {
                {"accountId", request.AccountId},
                {"apiToken", request.ApiToken},
                {"action", action},
                {"transactionId", request.TransactionId}
            };
            return values;
        }

        #endregion
    }
}