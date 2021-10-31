using System.Security.Cryptography;
using System.Text;

namespace MonamourWeb.Services.Encoding
{
    public class Md5Encoder : IEncodingService
    {
        public string GetHashCode(string value)
        {
            using (var md5 = MD5.Create())
            {
                var sb = new StringBuilder();
                var data = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));

                foreach (var t in data)
                    sb.Append(t.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}