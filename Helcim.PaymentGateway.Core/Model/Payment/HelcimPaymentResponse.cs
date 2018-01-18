using System.Collections.Generic;
using System.Xml.Serialization;

namespace Helcim.PaymentGateway.Core.Model.Payment
{
    public class HelcimMessageResponse
    {
        public HelcimMessageResponse()
        {
            Response = 0;
        }

        [XmlElement("response")]
        public int Response { get; set; }

        [XmlElement("responseMessage")]
        public string ResponseMessage { get; set; }
    }

    [XmlRoot("message")]
    public class HelcimPaymentResponse : HelcimMessageResponse
    {
        [XmlElement("notice")]
        public string Notice { get; set; }

        [XmlElement("transaction")]
        public HelcimPaymentTransaction Transaction { get; set; }
    }

    [XmlRoot("transactions")]
    public class HelcimTransactionsResponse : HelcimMessageResponse
    {
        public HelcimTransactionsResponse()
        {
            Transactions = new List<HelcimPaymentTransaction>();
        }

        [XmlElement("transaction")]
        public List<HelcimPaymentTransaction> Transactions { get; set; }
    }

}
