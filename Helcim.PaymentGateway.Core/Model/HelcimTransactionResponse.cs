using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helcim.PaymentGateway.Core.Model
{
    public class HelcimTransactionResponse
    {
        public List<HelcimTransaction> Transactions { get; set; }
    }
}
