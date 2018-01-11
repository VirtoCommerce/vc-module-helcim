using System;
using Helcim.PaymentGateway.Web.Managers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace Helcim.PaymentGateway.Web
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();
            Func<HelcimCheckoutPaymentMethod> helcimPaymentMethod = () =>
            {
                var paymentMethod = new HelcimCheckoutPaymentMethod();
                paymentMethod.Name = "Helcim Payment Gateway";
                paymentMethod.Description = "Helcim payment gateway integration";
                paymentMethod.LogoUrl = "https://raw.githubusercontent.com/VirtoCommerce/vc-module-helcim/master/Helcim.PaymentGateway.Web/Content/logo.svg";
                paymentMethod.Settings = settingsManager.GetModuleSettings("Helcim.PaymentGateway");
                return paymentMethod;
            };

            var paymentMethodsService = _container.Resolve<IPaymentMethodsService>();
            paymentMethodsService.RegisterPaymentMethod(helcimPaymentMethod);
        }
    }
}
