using System.Collections.Generic;

namespace Helcim.PaymentGateway.Core.Model
{
    public class HelcimTransactionRequest
    {
        public string AccountId { get; set; }

        public string ApiToken { get; set; }

        public string TransactionId { get; set; }

        public virtual Dictionary<string, string> ToDictionary()
        {
            var values = new Dictionary<string, string>
            {
                {"accountId", AccountId},
                {"apiToken", ApiToken},
                {"transactionId", TransactionId}
            };
            return values;
        }
    }
}
