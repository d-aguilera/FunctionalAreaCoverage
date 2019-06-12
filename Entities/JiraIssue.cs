using System;
using FunctionalAreaCoverage.Helpers;

namespace FunctionalAreaCoverage.Entities
{
    public abstract class JiraIssue
    {
        protected JiraIssue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Key = key;
        }

        public string Key { get; }
        // ReSharper disable once MemberCanBePrivate.Global
        public string Summary { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public string Status { get; set; }

        public virtual string ToCsv() => $"{Key}{CsvHelper.CsvSeparator}{Summary}{CsvHelper.CsvSeparator}{Status}";
    }
}
