using Helcim.PaymentGateway.Core.Model;

namespace Helcim.PaymentGateway.Core.Services
{
    public interface IHelcimCheckoutService
    {
        string GetCheckoutFormContent(HelcimCheckoutSettings settings);
    }
}
