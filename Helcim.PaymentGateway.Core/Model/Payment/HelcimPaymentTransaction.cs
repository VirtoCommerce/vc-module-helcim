using System.Xml.Serialization;

namespace Helcim.PaymentGateway.Core.Model.Payment
{
    public class HelcimPaymentTransaction
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("responseMessage")]
        public string ResponseMessage { get; set; }

        [XmlElement("transactionId")]
        public string TransactionId { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("time")]
        public string Time { get; set; }

        [XmlElement("cardHolderName")]
        public string CardHolderName { get; set; }

        [XmlElement("currency")]
        public string Currency { get; set; }

        [XmlElement("amount")]
        public decimal Amount { get; set; }

        [XmlElement("approvalCode")]
        public string ApprovalCode { get; set; }

        [XmlElement("avsResponse")]
        public string AvsResponse { get; set; }

        [XmlElement("cvvResponse")]
        public string CvvResponse { get; set; }

        [XmlElement("cardNumber")]
        public string CardNumber { get; set; }

        [XmlElement("cardToken")]
        public string CardToken { get; set; }

        [XmlElement("customerCode")]
        public string CustomerCode { get; set; }

        [XmlElement("orderNumber")]
        public string OrderNumber { get; set; }
    }
}
