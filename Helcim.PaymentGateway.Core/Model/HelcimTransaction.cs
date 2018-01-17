namespace Helcim.PaymentGateway.Core.Model
{
    public class HelcimTransaction
    {
        public string Id { get; set; }

        public string DateCreated { get; set; }

        public string Status { get; set; }

        public string User { get; set; }

        public string Type { get; set; }

        public string Amount { get; set; }

        public string ApprovalCode { get; set; }

        public string AvsResponse { get; set; }

        public string CvvResponse { get; set; }

        public string Test { get; set; }

        public string AcquirerTransactionId { get; set; }

        public string ResponseMessage { get; set; }

        public string IpAddress { get; set; }

        public string OrderNumber { get; set; }
    }
}
