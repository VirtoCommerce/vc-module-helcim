using System;
using System.Linq;
using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Model.Payment;
using Helcim.PaymentGateway.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace Helcim.PaymentGateway.Web.Services
{
    public class HelcimPaymentService : IHelcimPaymentService
    {
        private readonly string _apiEndpoint;
        private readonly IHelcimClient _helcimClient;

        #region Implementation of IHelcimPaymentService

        public HelcimPaymentService(string apiEndpoint, IHelcimClient client)
        {
            _apiEndpoint = apiEndpoint;
            _helcimClient = client;
        }

        public HelcimPaymentResponse GetTransaction(HelcimTransactionRequest request)
        {
            try
            {
                var values = request.ToDictionary();
                values.Add("action", "transactionView");

                var response = _helcimClient.MakeRequest(_apiEndpoint, values);
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

                var response = _helcimClient.MakeRequest(_apiEndpoint, values);
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

                var response = _helcimClient.MakeRequest(_apiEndpoint, values);
                return response.DeserializeXml<HelcimPaymentResponse>();
            }
            catch (Exception ex)
            {
                return new HelcimPaymentResponse {ResponseMessage = ex.Message};
            }
        }

        #endregion
    }
}