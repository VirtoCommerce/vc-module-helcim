using System.Collections.Generic;
using System.Collections.Specialized;
using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Model.Payment;
using Helcim.PaymentGateway.Core.Services;
using Helcim.PaymentGateway.Web.Managers;
using Moq;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.Settings;
using Xunit;

namespace Helcim.PaymentGateway.Tests
{
    public class HelcimPaymentMethodTests
    {
        [Fact]
        public void ProcessPaymentTest()
        {
            var checkoutService = new Mock<IHelcimCheckoutService>();
            checkoutService.Setup(x => x.GetCheckoutFormContent(It.IsAny<HelcimCheckoutSettings>())).Returns((HelcimCheckoutSettings setting) => "formcontent");
            var paymentService = new Mock<IHelcimPaymentService>();

            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("");

            var result = paymentMethod.ProcessPayment(new ProcessPaymentEvaluationContext()
            {
                Order = new CustomerOrder(),
                Store = new Store(),
                Payment = new PaymentIn()
            });

            Assert.True(result.IsSuccess);
            Assert.Equal(PaymentStatus.Pending, result.NewPaymentStatus);
            Assert.Equal("formcontent", result.HtmlForm);
        }

        [Fact]
        public void PostProcessPaymentTest_CallbackSuccessSale_PaymentProcessed()
        {
            var transactionId = "123";

            var checkoutService = new Mock<IHelcimCheckoutService>();
            var paymentService = new Mock<IHelcimPaymentService>();
            paymentService.Setup(x => x.GetTransaction(It.IsAny<HelcimTransactionRequest>())).Returns((HelcimTransactionRequest request) => new HelcimPaymentResponse()
            {
                Response = 1,
                Transaction = new HelcimPaymentTransaction()
                {
                    Id = transactionId,
                }
            });

            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("Sale");

            var context = new PostProcessPaymentEvaluationContext()
            {
                Payment = new PaymentIn { Transactions = new List<PaymentGatewayTransaction>() },
                Parameters = new NameValueCollection()
                {
                    {"response", "1"},
                    {"transactionId", transactionId}
                }
            };
            var result = paymentMethod.PostProcessPayment(context);

            Assert.True(result.IsSuccess);
            Assert.Equal(PaymentStatus.Paid, result.NewPaymentStatus);
            Assert.Equal(transactionId, result.OuterId);
            Assert.Equal(transactionId, context.Payment.OuterId);
            Assert.True(context.Payment.IsApproved);
        }

        [Fact]
        public void PostProcessPaymentTest_CallbackSuccessAuthorize_PaymentProcessed()
        {
            var transactionId = "123";

            var checkoutService = new Mock<IHelcimCheckoutService>();
            var paymentService = new Mock<IHelcimPaymentService>();
            paymentService.Setup(x => x.GetTransaction(It.IsAny<HelcimTransactionRequest>())).Returns((HelcimTransactionRequest request) => new HelcimPaymentResponse()
            {
                Response = 1,
                Transaction = new HelcimPaymentTransaction()
                {
                    Id = transactionId,
                }
            });

            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("Authorize");

            var context = new PostProcessPaymentEvaluationContext()
            {
                Payment = new PaymentIn { Transactions = new List<PaymentGatewayTransaction>() },
                Parameters = new NameValueCollection()
                {
                    {"response", "1"},
                    {"transactionId", transactionId}
                }
            };
            var result = paymentMethod.PostProcessPayment(context);

            Assert.True(result.IsSuccess);
            Assert.Equal(PaymentStatus.Authorized, result.NewPaymentStatus);
            Assert.Equal(transactionId, result.OuterId);
            Assert.Equal(transactionId, context.Payment.OuterId);
            Assert.False(context.Payment.IsApproved);
        }

        [Fact]
        public void PostProcessPaymentTest_CallbackError_PaymentNotProcessed()
        {
            var transactionId = "123";
            var responseMessage = "Invalid transaction";

            var checkoutService = new Mock<IHelcimCheckoutService>();
            var paymentService = new Mock<IHelcimPaymentService>();
            paymentService.Setup(x => x.GetTransaction(It.IsAny<HelcimTransactionRequest>())).Returns((HelcimTransactionRequest request) => new HelcimPaymentResponse()
            {
                Response = 1,
                Transaction = new HelcimPaymentTransaction()
                {
                    Id = transactionId,
                }
            });

            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("");

            var context = new PostProcessPaymentEvaluationContext()
            {
                Payment = new PaymentIn { Transactions = new List<PaymentGatewayTransaction>() },
                Parameters = new NameValueCollection()
                {
                    {"response", "0"},
                    {"responseMessage", responseMessage}
                }
            };
            var result = paymentMethod.PostProcessPayment(context);

            Assert.False(result.IsSuccess);
            Assert.Equal(responseMessage, result.ErrorMessage);
        }

        [Fact]
        public void PostProcessPaymentTest_CallbackSuccessInvalidTransaction_PaymentNotProcessed()
        {
            var responseMessage = "Invalid transaction";

            var checkoutService = new Mock<IHelcimCheckoutService>();
            var paymentService = new Mock<IHelcimPaymentService>();
            paymentService.Setup(x => x.GetTransaction(It.IsAny<HelcimTransactionRequest>())).Returns((HelcimTransactionRequest request) => new HelcimPaymentResponse()
            {
                ResponseMessage = responseMessage
            });

            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("");

            var context = new PostProcessPaymentEvaluationContext()
            {
                Payment = new PaymentIn { Transactions = new List<PaymentGatewayTransaction>() },
                Parameters = new NameValueCollection()
                {
                    {"response", "1"}
                }
            };
            var result = paymentMethod.PostProcessPayment(context);

            Assert.False(result.IsSuccess);
            Assert.Equal(responseMessage, result.ErrorMessage);
        }

        [Fact]
        public void CapturePaymentTest_ValidTransaction_PaymentCaptured()
        {
            var transactionId = "123";

            var checkoutService = new Mock<IHelcimCheckoutService>();
            var paymentService = new Mock<IHelcimPaymentService>();
            paymentService.Setup(x => x.CapturePayment(It.IsAny<HelcimTransactionRequest>())).Returns((HelcimTransactionRequest request) => new HelcimPaymentResponse()
            {
                Response = 1,
                Transaction = new HelcimPaymentTransaction() { TransactionId = transactionId }
            });

            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("");

            var context = new CaptureProcessPaymentEvaluationContext()
            {
                Payment = new PaymentIn()
                {
                    PaymentStatus = PaymentStatus.Authorized,
                    OuterId = transactionId,
                    Transactions = new List<PaymentGatewayTransaction>()
                }
            };
            var result = paymentMethod.CaptureProcessPayment(context);

            Assert.True(result.IsSuccess);
            Assert.True(context.Payment.IsApproved);
            Assert.Equal(PaymentStatus.Paid, result.NewPaymentStatus);
            Assert.Equal(PaymentStatus.Paid, context.Payment.PaymentStatus);
        }

        [Fact]
        public void CapturePaymentTest_InvalidTransaction_PaymentNotCaptured()
        {
            var responseMessage = "Invalid transaction";

            var checkoutService = new Mock<IHelcimCheckoutService>();
            var paymentService = new Mock<IHelcimPaymentService>();
            paymentService.Setup(x => x.CapturePayment(It.IsAny<HelcimTransactionRequest>())).Returns((HelcimTransactionRequest request) => new HelcimPaymentResponse()
            {
                ResponseMessage = responseMessage
            });

            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("");

            var context = new CaptureProcessPaymentEvaluationContext()
            {
                Payment = new PaymentIn()
                {
                    PaymentStatus = PaymentStatus.Authorized,
                    Transactions = new List<PaymentGatewayTransaction>()
                }
            };
            var result = paymentMethod.CaptureProcessPayment(context);

            Assert.False(result.IsSuccess);
            Assert.Equal(responseMessage, result.ErrorMessage);
        }

        [Fact]
        public void ValidatePostProcessRequestTest_ValidTransaction_RequestValid()
        {
            var transactionId = "123";
            var code = "HelcimCheckout";

            var checkoutService = new Mock<IHelcimCheckoutService>();
            var paymentService = new Mock<IHelcimPaymentService>();
            var paymentMethod = new HelcimCheckoutPaymentMethod(checkoutService.Object, s => paymentService.Object);
            paymentMethod.Settings = GetTestSettings("");

            var result = paymentMethod.ValidatePostProcessRequest(new NameValueCollection()
            {
                { "transactionId", transactionId },
                { "code", code }
            });

            Assert.True(result.IsSuccess);
            Assert.Equal(transactionId, result.OuterId);
        }

        private List<SettingEntry> GetTestSettings(string paymentType)
        {
            return new List<SettingEntry>()
            {
                new SettingEntry { Name = "Helcim.PaymentGateway.PaymentType", Value = paymentType },
                new SettingEntry { Name = "Helcim.PaymentGateway.Mode", Value = "Test" },
                new SettingEntry { Name = "Helcim.PaymentGateway.FormAction", Value = "Test" },
                new SettingEntry { Name = "Helcim.PaymentGateway.Token", Value = "Test" },
                new SettingEntry { Name = "Helcim.PaymentGateway.SecretKey", Value = "Test" },
                new SettingEntry { Name = "Helcim.PaymentGateway.ApiToken", Value = "Test" },
                new SettingEntry { Name = "Helcim.PaymentGateway.AccountId", Value = "Test" },
                new SettingEntry { Name = "Helcim.PaymentGateway.ApiEndpoint", Value = "Test" },
                new SettingEntry { Name = "Helcim.PaymentGateway.HelcimjsPath", Value = "Test"}
            };
        }
    }
}
