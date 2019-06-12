using System;
using System.IO;

namespace FunctionalAreaCoverage.Helpers
{
    internal static class CacheHelper
    {
        public const string FunctionalAreasKey = "functional-areas";
        public const string EndToEndsKey = "end-to-ends";

        public static string Get(string key, Func<string> callback)
        {
            if (!ConfigurationHelper.CacheEnabled)
            {
                return callback();
            }

            string value;

            try
            {
                value = File.ReadAllText(ToFileKey(key));
                TraceHelper.WriteLine($"'{key}' found in cache");
            }
            catch (FileNotFoundException)
            {
                TraceHelper.WriteLine($"'{key}' not found in cache");
                value = callback();
                Put(key, value);
            }

            return value;
        }

        private static void Put(string key, string value)
        {
            if (!ConfigurationHelper.CacheEnabled)
            {
                return;
            }

            File.WriteAllText(ToFileKey(key), value);

            TraceHelper.WriteLine($"'{key}' saved in cache");
        }

        private static string ToFileKey(string key) => $"{key}.cache";
    }
}
