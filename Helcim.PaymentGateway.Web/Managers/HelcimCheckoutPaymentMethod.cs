using System;
using System.Collections.Specialized;
using VirtoCommerce.Domain.Payment.Model;

namespace Helcim.PaymentGateway.Web.Managers
{
    public class HelcimCheckoutPaymentMethod : PaymentMethod
    {
        private const string _paymentMethodModeStoreSetting = "Helcim.PaymentGateway.Mode";

        #region Settings        

        private string ApiMode
        {
            get
            {
                return GetSetting(_paymentMethodModeStoreSetting);
            }
        }
        
        #endregion

        public override PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.PreparedForm; }
        }

        public override PaymentMethodGroupType PaymentMethodGroupType
        {
            get { return PaymentMethodGroupType.Alternative; }
        }

        public HelcimCheckoutPaymentMethod() : base("HelcimCheckout")
        {
        }

        public override ProcessPaymentResult ProcessPayment(ProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override RefundProcessPaymentResult RefundProcessPayment(RefundProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString)
        {
            throw new NotImplementedException();
        }

        public override VoidProcessPaymentResult VoidProcessPayment(VoidProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}