using System.Linq;
using System.Text.RegularExpressions;

namespace FunctionalAreaCoverage.Helpers
{
    internal static class CamelHelper
    {
        private static readonly Regex CamelRegex = new Regex(@"(?<=[a-z])([A-Z])", RegexOptions.Compiled);

        public static string ToProperName(string arg)
        {
            var words = CamelRegex.Replace(arg, @" $1")
                .Split(' ')
                .Select(HandleAcronyms);
            return string.Join(" ", words);
        }

        private static string HandleAcronyms(string arg)
        {
            var upper = arg.ToUpperInvariant();
            switch (upper)
            {
                case "QB":
                case "QE":
                case "TC":
                case "E2E":
                    return upper;

                default:
                    return arg;
            }
        }
    }
}
