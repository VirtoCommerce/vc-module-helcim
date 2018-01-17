using System.Threading.Tasks;
using Helcim.PaymentGateway.Core.Model;

namespace Helcim.PaymentGateway.Core.Services
{
    public interface IHelcimService
    {
        Task<HelcimTransactionResponse> GetTransaction(HelcimRequest request);

        Task<HelcimPaymentResponse> CapturePayment(HelcimRequest request);
    }
}
