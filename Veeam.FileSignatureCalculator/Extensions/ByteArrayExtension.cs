using System.Text;

namespace Veeam.FileSignatureCalculator.Extensions
{
    /// <summary>
    /// byte[] extensions
    /// </summary>
    public static class ByteArrayExtension
    {
        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] data)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}