using System;
using System.Collections.Specialized;
using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Platform.Core.Common;

namespace Helcim.PaymentGateway.Web.Managers
{
    public class HelcimCheckoutPaymentMethod : PaymentMethod
    {
        private const string _paymentMethodModeStoreSetting = "Helcim.PaymentGateway.Mode";
        private const string _formActionUrlSetting = "Helcim.PaymentGateway.FormAction";
        private const string _tokenSetting = "Helcim.PaymentGateway.Token";
        private const string _secretKeySetting = "Helcim.PaymentGateway.SecretKey";
        private const string _apiAccessToken = "Helcim.PaymentGateway.ApiToken";
        private const string _accountId = "Helcim.PaymentGateway.AccountId";

        private const string _apiEndpoint = "Helcim.PaymentGateway.ApiEndpoint";

        #region Settings        

        private string ApiMode
        {
            get
            {
                return GetSetting(_paymentMethodModeStoreSetting);
            }
        }

        private string FormAction
        {
            get
            {
                return GetSetting(_formActionUrlSetting);
            }
        }

        private string Token
        {
            get
            {
                return GetSetting(_tokenSetting);
            }
        }
        private string SecretKey
        {
            get
            {
                return GetSetting(_secretKeySetting);
            }
        }

        private string AccountId
        {
            get
            {
                return GetSetting(_accountId);
            }
        }

        private string ApiAccessToken
        {
            get
            {
                return GetSetting(_apiAccessToken);
            }
        }

        private string ApiEndpoint
        {
            get
            {
                return GetSetting(_apiEndpoint);
            }
        }

        public bool IsTest
        {
            get { return ApiMode.EqualsInvariant("test"); }
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

        private readonly IHelcimCheckoutService _checkoutService;
        private readonly Func<string, IHelcimService> _helcimServiceFactory;

        public HelcimCheckoutPaymentMethod(IHelcimCheckoutService checkoutService, Func<string, IHelcimService> helcimServiceFactory) 
            : base("HelcimCheckout")
        {
            _checkoutService = checkoutService;
            _helcimServiceFactory = helcimServiceFactory;
        }

        public override ProcessPaymentResult ProcessPayment(ProcessPaymentEvaluationContext context)
        {
            var result = new ProcessPaymentResult();
            if (context.Order != null && context.Store != null)
            {
                result = PrepareFormContent(context);
            }
            return result;
        }

        private ProcessPaymentResult PrepareFormContent(ProcessPaymentEvaluationContext context)
        {
            var formContent = _checkoutService.GetCheckoutFormContent(new HelcimCheckoutSettings
            {
                Order = context.Order,
                IsTest = IsTest,
                FormAction = FormAction,
                Token = Token,
                SecretKey = SecretKey,
                PaymentMethodCode = "HelcimCheckout"
            });

            var result = new ProcessPaymentResult();
            result.IsSuccess = true;
            result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Pending;
            result.HtmlForm = formContent;
            result.OuterId = null;

            return result;
        }


        public override PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context)
        {
            var result = new PostProcessPaymentResult();

            var response = GetParamValue(context.Parameters, "response");
            var transactionId = GetParamValue(context.Parameters, "transactionId");
            var cardToken = GetParamValue(context.Parameters, "cardToken");

            var transactionSuccess = response.EqualsInvariant("1");
            if (transactionSuccess)
            {
                //double check transaction 
                var service = _helcimServiceFactory(ApiEndpoint);
                var transactionResult = service.GetTransaction(new HelcimRequest
                {
                    AccountId = AccountId,
                    ApiToken = ApiAccessToken,
                    TransactionId = transactionId
                });

                if (transactionResult != null)
                {
                    result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Paid;
                    context.Payment.OuterId = result.OuterId = transactionId;
                    context.Payment.AuthorizedDate = DateTime.UtcNow;
                    //context.Payment.CapturedDate = DateTime.UtcNow;
                    context.Payment.IsApproved = true;
                    result.IsSuccess = true;
                }
            }
            else
            {
                result.ErrorMessage = GetParamValue(context.Parameters, "error");
            }

            return result;
        }

        public override CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override RefundProcessPaymentResult RefundProcessPayment(RefundProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override VoidProcessPaymentResult VoidProcessPayment(VoidProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check for transaction Id and payment method code in returned params collection
        /// This method is repsonsible for selecting this particular payment method among all active store payment methods
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString)
        {
            var transactionId = GetParamValue(queryString, "transactionId");
            var code = GetParamValue(queryString, "code");
            
            return new ValidatePostProcessRequestResult
            {
                IsSuccess = code.EqualsInvariant(Code),
                OuterId = transactionId            
            };
        }

        private string GetParamValue(NameValueCollection queryString, string paramName)
        {
            if (queryString == null || !queryString.HasKeys())
            {
                return null;
            }

            return queryString.Get(paramName);
        }
    }
}