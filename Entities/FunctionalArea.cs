using System.Collections.Generic;
using FunctionalAreaCoverage.Helpers;

namespace FunctionalAreaCoverage.Entities
{
    public class FunctionalArea : JiraIssue
    {
        public static readonly string CsvHeader = $"{nameof(Key)}{CsvHelper.CsvSeparator}{nameof(Summary)}{CsvHelper.CsvSeparator}{nameof(Status)}";

        public FunctionalArea(string key) : base(key)
        {
        }

        public IEnumerable<EndToEnd> CoveredBy { get; set; }
    }
}
