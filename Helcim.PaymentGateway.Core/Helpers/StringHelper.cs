using System.Security.Cryptography;
using System.Text;

namespace Helcim.PaymentGateway.Core.Helpers
{
    public static class StringHelper
    {
        public static string ToSHA256Hash(this string source)
        {
            var stringBuilder = new StringBuilder();

            var sha256 = new SHA256Managed();
            var computed = sha256.ComputeHash(Encoding.UTF8.GetBytes(source));
            foreach (var b in computed)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public static string ToCurrencyString(this decimal source)
        {
            return source.ToString("N2");
        }
    }
}
