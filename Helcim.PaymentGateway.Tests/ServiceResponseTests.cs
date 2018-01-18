using System.IO;
using System.Linq;
using Helcim.PaymentGateway.Core.Model.Payment;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace Helcim.PaymentGateway.Tests
{
    public class ServiceResponseTests
    {
        [Fact]
        public void GetTransaction_SuccessfulReponse_ResponseRead()
        {
            //arrange
            var response = File.ReadAllText(@"../../Data/transactionResponse.xml");

            //act
            var trasnsaction = response.DeserializeXml<HelcimTransactionsResponse>();

            //assert
            Assert.Equal("APPROVED", trasnsaction.Transactions.FirstOrDefault().Status);
        }

        [Fact]
        public void Capture_ErrorReponse_ResponseRead()
        {
            //arrange
            var response = File.ReadAllText(@"../../Data/transactionErrorResponse.xml");

            //act
            var payment = response.DeserializeXml<HelcimPaymentResponse>();

            //assert
            Assert.Equal(0, payment.Response);
        }

        [Fact]
        public void Capture_SuccessfulReponse_ResponseRead()
        {
            //arrange
            var response = File.ReadAllText(@"../../Data/captureResponse.xml");

            //act
            var payment = response.DeserializeXml<HelcimPaymentResponse>();

            //assert
            Assert.Equal(1, payment.Response);
        }

        [Fact]
        public void Capture_ErrorOrSuccessReponse_ResponseRead()
        {
            //arrange
            var response = File.ReadAllText(@"../../Data/captureResponse.xml");

            //act
            var payment = response.DeserializeXml<HelcimPaymentResponse>();

            //assert
            Assert.Equal(1, payment.Response);
        }
    }
}
