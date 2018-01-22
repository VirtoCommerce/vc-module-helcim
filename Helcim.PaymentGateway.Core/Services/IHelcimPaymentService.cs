using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Model.Payment;

namespace Helcim.PaymentGateway.Core.Services
{
    public interface IHelcimPaymentService
    {
        HelcimPaymentResponse GetTransaction(HelcimTransactionRequest request);

        HelcimPaymentResponse CapturePayment(HelcimTransactionRequest request);

        HelcimPaymentResponse CheckTransaction(HelcimCheckTransactionRequest request);
    }
}
