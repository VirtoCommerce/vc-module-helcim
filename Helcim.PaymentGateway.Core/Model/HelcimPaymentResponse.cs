using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helcim.PaymentGateway.Core.Model
{
    public class HelcimPaymentResponse
    {
        public HelcimPaymentMessage Message { get; set; }
    }

    public class HelcimPaymentMessage
    {
        public int Response { get; set; }

        public string ResponseMessage { get; set; }

        public string Notice { get; set; }

        public HelcimTransaction Transaction { get; set; }
    }
}
