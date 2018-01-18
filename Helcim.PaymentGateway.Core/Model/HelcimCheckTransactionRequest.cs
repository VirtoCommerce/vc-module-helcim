using System.Collections.Generic;
using Helcim.PaymentGateway.Core.Helpers;

namespace Helcim.PaymentGateway.Core.Model
{
    public class HelcimCheckTransactionRequest : HelcimTransactionRequest
    {
        public string OrderNumber { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        #region Overrides of HelcimTransactionRequest

        public override Dictionary<string, string> ToDictionary()
        {
            var result = base.ToDictionary();

            result.Add("orderNumber", OrderNumber);
            result.Add("amount", Amount.ToCurrencyString());
            result.Add("currency", Currency);

            return result;
        }

        #endregion
    }
}