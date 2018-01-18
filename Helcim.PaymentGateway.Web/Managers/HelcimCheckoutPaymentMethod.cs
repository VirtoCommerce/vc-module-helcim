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
        private const string _paymentActionType = "Helcim.PaymentGateway.PaymentType";

        private const string _tokenSetting = "Helcim.PaymentGateway.Token";
        private const string _secretKeySetting = "Helcim.PaymentGateway.SecretKey";

        private const string _apiAccessToken = "Helcim.PaymentGateway.ApiToken";
        private const string _accountId = "Helcim.PaymentGateway.AccountId";

        private const string _apiEndpoint = "Helcim.PaymentGateway.ApiEndpoint";
        private const string _helcimJsPath = "Helcim.PaymentGateway.HelcimjsPath";

        #region Settings        

        /// <summary>
        /// Test or Live
        /// </summary>
        private string ApiMode => GetSetting(_paymentMethodModeStoreSetting);

        /// <summary>
        /// Storefront order creation endpoint 
        /// </summary>
        private string FormAction => GetSetting(_formActionUrlSetting);

        private string Token => GetSetting(_tokenSetting);

        private string SecretKey => GetSetting(_secretKeySetting);

        private string AccountId => GetSetting(_accountId);

        private string ApiAccessToken => GetSetting(_apiAccessToken);

        private string ApiEndpoint => GetSetting(_apiEndpoint);

        private string HelcimJsPath => GetSetting(_helcimJsPath);

        public string PaymentActionType => GetSetting(_paymentActionType);

        public bool IsTest => ApiMode.EqualsInvariant("test");

        #endregion

        public override PaymentMethodType PaymentMethodType => PaymentMethodType.PreparedForm;

        public override PaymentMethodGroupType PaymentMethodGroupType => PaymentMethodGroupType.Alternative;

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
                PaymentMethodCode = Code,
                HelcimJsPath = HelcimJsPath
            });

            var result = new ProcessPaymentResult
            {
                IsSuccess = true,
                NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Pending,
                HtmlForm = formContent,
                OuterId = null
            };

            return result;
        }

        public override PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context)
        {
            var result = new PostProcessPaymentResult();

            var response = GetParamValue(context.Parameters, "response");
            var transactionId = GetParamValue(context.Parameters, "transactionId");

            var transactionSuccess = response.EqualsInvariant("1");
            if (!transactionSuccess)
            {
                result.ErrorMessage = GetParamValue(context.Parameters, "responseMessage");
                return result;
            }

            //double check transaction status to be sure it legit came from helcim
            var service = _helcimServiceFactory(ApiEndpoint);
            var transactionResult = service.GetTransaction(new HelcimTransactionRequest()
            {
                AccountId = AccountId,
                ApiToken = ApiAccessToken,
                TransactionId = transactionId
            });

            if (transactionResult.Response != 1)
            {
                result.ErrorMessage = transactionResult.ResponseMessage;
                return result;
            }

            if (PaymentActionType == "Sale")
            {
                result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Paid;
                context.Payment.CapturedDate = DateTime.UtcNow;
                context.Payment.IsApproved = true;
            }
            else
            {
                result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Authorized;
            }

            context.Payment.AuthorizedDate = DateTime.UtcNow;
            context.Payment.OuterId = result.OuterId = transactionId;
            result.IsSuccess = true;

            return result;
        }

        public override CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context)
        {
            if (context == null || context.Payment == null)
                throw new ArgumentNullException(nameof(context));

            var result = new CaptureProcessPaymentResult();

            if (context.Payment.IsApproved || (context.Payment.PaymentStatus != PaymentStatus.Authorized &&
                                               context.Payment.PaymentStatus != PaymentStatus.Cancelled))
            {
                return result;
            }

            var paymentService = _helcimServiceFactory(ApiEndpoint);
            var captureResult = paymentService.CapturePayment(new HelcimTransactionRequest
            {
                AccountId = AccountId,
                ApiToken = ApiAccessToken,
                TransactionId = context.Payment.OuterId
            });

            //check response
            if (captureResult.Response != 1)
            {
                result.ErrorMessage = captureResult.ResponseMessage;
                return result;
            }

            result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Paid;
            context.Payment.CapturedDate = DateTime.UtcNow;
            result.IsSuccess = true;
            context.Payment.IsApproved = true;
            return result;
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