using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Model.Payment;
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

        public HelcimPaymentResponse GetTransaction(HelcimTransactionRequest request)
        {
            try
            {
                var values = request.ToDictionary();
                values.Add("action", "transactionView");

                var response = MakeCall(values);
                if (response.Contains("<message>"))
                {
                    return response.DeserializeXml<HelcimPaymentResponse>();
                }

                var transactionResponse = response.DeserializeXml<HelcimTransactionsResponse>();
                var transaction = transactionResponse.Transactions.FirstOrDefault();
                if (transaction == null)
                {
                    return new HelcimPaymentResponse { ResponseMessage = "Empty transaction in response" };
                }

                return new HelcimPaymentResponse()
                {
                    Transaction = transaction,
                    Response = transaction.Status.EqualsInvariant("APPROVED") ? 1 : 0,
                    ResponseMessage = transaction.ResponseMessage
                };
            }
            catch (Exception ex)
            {
                return new HelcimPaymentResponse { ResponseMessage = ex.Message };
            }
        }

        public HelcimPaymentResponse CapturePayment(HelcimTransactionRequest request)
        {
            try
            {
                var values = request.ToDictionary();
                values.Add("transactionType", "capture");

                var response = MakeCall(values);
                return response.DeserializeXml<HelcimPaymentResponse>();
            }
            catch (Exception ex)
            {
                return new HelcimPaymentResponse { ResponseMessage = ex.Message };
            }
        }

        public HelcimPaymentResponse CheckTransaction(HelcimCheckTransactionRequest request)
        {
            try
            {
                var values = request.ToDictionary();
                values.Add("transactionOtherType", "check");

                var response = MakeCall(values);
                return response.DeserializeXml<HelcimPaymentResponse>();
            }
            catch (Exception ex)
            {
                return new HelcimPaymentResponse {ResponseMessage = ex.Message};
            }
        }

        private string MakeCall(Dictionary<string, string> values)
        {
            var requestContent = new FormUrlEncodedContent(values);
            var postTask = Task.Run(async () => await _httpClient.PostAsync(_apiEndpoint, requestContent));
            var responseMessage = postTask.Result;

            var readTask = Task.Run(async () => await responseMessage.Content.ReadAsStringAsync());
            var resultContent = readTask.Result;
            return resultContent;
        }

        #endregion
    }
}