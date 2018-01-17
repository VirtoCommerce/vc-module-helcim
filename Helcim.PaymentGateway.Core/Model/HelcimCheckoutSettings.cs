using VirtoCommerce.Domain.Order.Model;

namespace Helcim.PaymentGateway.Core.Model
{
    public class HelcimCheckoutSettings
    {
        public CustomerOrder Order { get; set; } 

        public bool IsTest { get; set; }

        public string FormAction { get; set; }

        public string Token { get; set; }

        public string SecretKey { get; set; }

        public string Currency { get; set; }

        public string Language { get; set; }

        public string PaymentMethodCode { get; set; }
    }
}
