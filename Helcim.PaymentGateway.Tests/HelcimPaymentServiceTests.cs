using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Helcim.PaymentGateway.Core.Model;
using Helcim.PaymentGateway.Core.Services;
using Helcim.PaymentGateway.Web.Services;
using Moq;
using Xunit;

namespace Helcim.PaymentGateway.Tests
{
    public class HelcimPaymentServiceTests
    {
        [Fact]
        public void CallGetTransaction_TransactionPresent_SuccessfulResult()
        {
            var responseMessage = "APPROVED";
            var transactionId = "111222";
            var mockCallResult = $@"<transactions>
                                        <transaction>
                                            <id>{transactionId}</id>
                                            <status>{responseMessage}</status>
                                            <responseMessage>{responseMessage}</responseMessage>
                                        </transaction>
                                    </transactions>";

            var mockClient = new Mock<IHelcimClient>();
            mockClient
                .Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns((string endpint, Dictionary<string, string> values) => mockCallResult);

            var paymentService = new HelcimPaymentService("testendpoint", mockClient.Object);

            var result = paymentService.GetTransaction(new HelcimCheckTransactionRequest());

            Assert.False(result.Error);
            Assert.Equal(responseMessage, result.ResponseMessage);
            Assert.Equal(transactionId, result.Transaction.Id);
        }

        [Fact]
        public void CallGetTransaction_TransactionNotPresent_ErrorResult()
        {
            var response = "0";
            var responseMessage = "Invalid Transaction ID";
            var mockCallResult = $@"<message>
                                        <response>{response}</response>
                                        <responseMessage>{responseMessage}</responseMessage>
                                   </message>";

            var mockClient = new Mock<IHelcimClient>();
            mockClient
                .Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns((string endpint, Dictionary<string, string> values) => mockCallResult);
            var paymentService = new HelcimPaymentService("testendpoint", mockClient.Object);

            var result = paymentService.GetTransaction(new HelcimCheckTransactionRequest());

            Assert.True(result.Error);
            Assert.Equal(responseMessage, result.ResponseMessage);
            Assert.Null(result.Transaction);
        }

        [Fact]
        public void CallCaptureTransaction_TransactionPresent_SuccessfulResult()
        {
            var response = "1";
            var transactionId = "111222";
            var responseMessage = "APPROVED";
            var mockCallResult = $@"<message>
                                        <response>{response}</response>
                                        <responseMessage>{responseMessage}</responseMessage>
                                        <transaction>
                                            <transactionId>{transactionId}</transactionId>
                                        </transaction>
                                    </message>";

            var mockClient = new Mock<IHelcimClient>();
            mockClient
                .Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns((string endpint, Dictionary<string, string> values) => mockCallResult);
            var paymentService = new HelcimPaymentService("testendpoint", mockClient.Object);

            var result = paymentService.CapturePayment(new HelcimCheckTransactionRequest());

            Assert.False(result.Error);
            Assert.Equal(responseMessage, result.ResponseMessage);
            Assert.Equal(transactionId, result.Transaction.TransactionId);
        }

        [Fact]
        public void CallCaptureTransaction_TransactionNotPresent_ErrorResult()
        {
            var response = "0";
            var responseMessage = "Invalid Transaction ID";
            var mockCallResult = $@"<message>
                                        <response>{response}</response>
                                        <responseMessage>{responseMessage}</responseMessage>
                                    </message>";

            var mockClient = new Mock<IHelcimClient>();
            mockClient
                .Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns((string endpint, Dictionary<string, string> values) => mockCallResult);
            var paymentService = new HelcimPaymentService("testendpoint", mockClient.Object);

            var result = paymentService.CapturePayment(new HelcimCheckTransactionRequest());

            Assert.True(result.Error);
            Assert.Equal(responseMessage, result.ResponseMessage);
            Assert.Null(result.Transaction);
        }

        [Fact]
        public void CallGetTransaction_CommunicationError_ExceptionCaptured()
        {
            var message = "Exception message";

            var mockClient = new Mock<IHelcimClient>();
            mockClient
                .Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Throws(new Exception(message));
            var paymentService = new HelcimPaymentService("testendpoint", mockClient.Object);

            var result = paymentService.GetTransaction(new HelcimCheckTransactionRequest());

            Assert.True(result.Error);
            Assert.Equal(message, result.ResponseMessage);
            Assert.Null(result.Transaction);
        }

        [Fact]
        public void CallCaptureTransaction_CommunicationError_ExceptionCaptured()
        {
            var message = "Exception message";

            var mockClient = new Mock<IHelcimClient>();
            mockClient
                .Setup(x => x.MakeRequest(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Throws(new Exception(message));
            var paymentService = new HelcimPaymentService("testendpoint", mockClient.Object);

            var result = paymentService.CapturePayment(new HelcimCheckTransactionRequest());

            Assert.True(result.Error);
            Assert.Equal(message, result.ResponseMessage);
            Assert.Null(result.Transaction);
        }
    }
}
