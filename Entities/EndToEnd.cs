using FunctionalAreaCoverage.Helpers;

namespace FunctionalAreaCoverage.Entities
{
    public class EndToEnd : JiraIssue
    {
        public static readonly string CsvHeader = $"{nameof(Key)}{CsvHelper.CsvSeparator}{nameof(Summary)}{CsvHelper.CsvSeparator}{nameof(Status)}";

        public EndToEnd(string key) : base(key)
        {
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string TestSuiteCategory { get; set; }
        
        public override string ToCsv() => $"{base.ToCsv()}{CsvHelper.CsvSeparator}{TestSuiteCategory}";
    }
}