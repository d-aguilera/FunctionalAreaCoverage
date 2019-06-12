using System;
using System.Text;

namespace FunctionalAreaCoverage.Helpers
{
    internal static class Base64Helper
    {
        public static string Encode(string plainText) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
    }
}
