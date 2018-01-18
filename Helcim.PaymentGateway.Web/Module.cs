using System;
using System.Linq;
using System.Net.Http;
using Helcim.PaymentGateway.Core.Services;
using Helcim.PaymentGateway.Web.Managers;
using Helcim.PaymentGateway.Web.Services;
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
            _container.RegisterInstance("HelcimHttpClinet", new HttpClient());
            Func<string, IHelcimService> helcimServiceFactory = endpoint => new HelcimService(endpoint, _container.Resolve<HttpClient>("HelcimHttpClinet"));
            _container.RegisterInstance(helcimServiceFactory);

            _container.RegisterType<IHelcimCheckoutService, HelcimCheckoutService>();
            
            var settingsManager = ServiceLocator.Current.GetInstance<ISettingsManager>();
            Func<HelcimCheckoutPaymentMethod> helcimPaymentMethod = () =>
            {
                var paymentMethod = new HelcimCheckoutPaymentMethod(_container.Resolve<IHelcimCheckoutService>(), _container.Resolve<Func<string, IHelcimService>>());
                paymentMethod.Name = "Helcim Payment Gateway";
                paymentMethod.Description = "Helcim payment gateway integration";
                paymentMethod.LogoUrl = "https://raw.githubusercontent.com/VirtoCommerce/vc-module-helcim/master/Helcim.PaymentGateway.Web/Content/logo.svg";
                paymentMethod.Settings = settingsManager.GetModuleSettings("Helcim.PaymentGateway").OrderBy(x=>x.Title).ToArray();
                return paymentMethod;
            };

            var paymentMethodsService = _container.Resolve<IPaymentMethodsService>();
            paymentMethodsService.RegisterPaymentMethod(helcimPaymentMethod);
        }
    }
}
