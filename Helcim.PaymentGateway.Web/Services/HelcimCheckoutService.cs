using System;
using System.IO;
using System.Reflection;
using DotLiquid;
using Helcim.PaymentGateway.Core.Helpers;
using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Services;

namespace Helcim.PaymentGateway.Web.Services
{
    public class HelcimCheckoutService : IHelcimCheckoutService
    {
        #region Implementation of IHelcimCheckoutService

        public string GetCheckoutFormContent(HelcimCheckoutSettings settings)
        {
            var formContent = GetFormContent();

            var amount = settings.Order.Sum.ToCurrencyString();
            var amountHash = (settings.SecretKey + amount).ToSHA256Hash();

            Template template = Template.Parse(formContent);
            var content = template.Render(Hash.FromAnonymousObject(new
            {
                formAction = settings.FormAction,
                isTest = Convert.ToInt32(settings.IsTest),
                token = settings.Token,
                orderNumber = settings.Order.Number,
                orderId = settings.Order.Id,
                paymentMethodCode = settings.PaymentMethodCode,
                amount,
                amountHash,
                helcimJsPath = settings.HelcimJsPath
            }));

            return content;
        }

        private static string GetFormContent()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Helcim.PaymentGateway.Web.Content.paymentForm.liquid");
            if (stream == null)
            {
                return String.Empty;
            }

            string formContent;
            using (StreamReader sr = new StreamReader(stream))
            {
                formContent = sr.ReadToEnd();
            }
            return formContent;
        }

        #endregion
    }
}
