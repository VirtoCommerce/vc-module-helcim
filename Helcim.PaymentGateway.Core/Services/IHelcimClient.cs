using System.Collections.Generic;

namespace Helcim.PaymentGateway.Core.Services
{
    public interface IHelcimClient
    {
        string MakeRequest(string endpoint, Dictionary<string, string> requestValues);
    }
}
