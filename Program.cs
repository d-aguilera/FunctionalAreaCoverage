using FunctionalAreaCoverage.Entities;
using FunctionalAreaCoverage.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FunctionalAreaCoverage
{
    internal static class Program
    {
        private static void Main()
        {
            TraceHelper.Info("Functional Area Coverage");
            TraceHelper.Info();

            new[] {"ProjectName", "CacheEnabled", "SpreadsheetId"}
                .ToDictionary(key => key, ConfigurationHelper.GetSetting)
                .Select(kvp => $"{kvp.Key} = {kvp.Value}")
                .ToList()
                .ForEach(TraceHelper.Info);
            TraceHelper.Info();

            TraceHelper.Run(() => MainImpl());

            TraceHelper.Info("Finished.");
        }

        private static void MainImpl()
        {
            TraceHelper.Info("Fetching information from Jira...");

            var data = JiraHelper.GetFunctionalAreas().ToList();

            TraceHelper.Info("Done.");

            var tasks = new[]
            {
                new Task(CreateFunctionalAreasData, "FA.csv", "FA_DATA"),
                new Task(CreateEndToEndsData, "E2E.csv", "E2E_DATA"),
                new Task(CreateFunctionalAreasEndToEndsData, "FA-E2E.csv", "FAE2E_DATA"),
            };

            foreach (var task in tasks)
            {
                task.Method(data, out var csv, out var header);
                SaveCsvFile(task.Path, header, csv);
                UpdateSheetRange(task.NamedRange, csv);
            }

            SheetsHelper.AddFillerData();
        }

        private static void SaveCsvFile(string path, IEnumerable<string> header, IEnumerable<string> csv)
        {
            TraceHelper.Run(() => SaveCsvFileImpl(path, header, csv));
        }

        private static void SaveCsvFileImpl(string path, IEnumerable<string> header, IEnumerable<string> csv)
        {
            var lines = header.Concat(csv).ToArray();
            var contents = string.Join(Environment.NewLine, lines);
            File.WriteAllText(path, contents);
            TraceHelper.Info($"{path} created successfully ({lines.Length - 1} records)");
        }

        private static void UpdateSheetRange(string rangeName, IEnumerable<string> csv)
        {
            TraceHelper.Run(() => UpdateSheetRangeImpl(rangeName, csv));
        }

        private static void UpdateSheetRangeImpl(string rangeName, IEnumerable<string> csv)
        {
            TraceHelper.Info($"Updating spreadsheet ({rangeName})...");
            var data = csv.Select(line => line.Split(CsvHelper.CsvSeparator));
            SheetsHelper.UpdateRange(rangeName, data);
        }

        private static void CreateFunctionalAreasData(IEnumerable<FunctionalArea> data, out string[] csv, out string[] header)
        {
            header = new[] { FunctionalArea.CsvHeader };
            csv = data
                .OrderBy(fa => fa.Key)
                .Select(fa => fa.ToCsv())
                .ToArray();
        }

        private static void CreateEndToEndsData(IEnumerable<FunctionalArea> data, out string[] csv, out string[] header)
        {
            header = new[] { EndToEnd.CsvHeader };
            csv = data
                .SelectMany(fa => fa.CoveredBy)
                .Distinct(new JiraIssueComparer())
                .OrderBy(e => e.Key)
                .Select(e => e.ToCsv())
                .ToArray();
        }

        private static void CreateFunctionalAreasEndToEndsData(IEnumerable<FunctionalArea> data, out string[] csv, out string[] header)
        {
            header = new[] { FunctionalAreaEndToEndRelation.CsvHeader };
            csv = data
                .SelectMany(fa => fa.CoveredBy
                    .Select(e => new FunctionalAreaEndToEndRelation(fa.Key, e.Key)))
                .Distinct(new RelationComparer())
                .OrderBy(r => r.Key1)
                .ThenBy(r => r.Key2)
                .Select(r => r.ToCsv())
                .ToArray();
        }

        private delegate void CreateDataDelegate(IEnumerable<FunctionalArea> data, out string[] csv, out string[] header);

        private class Task
        {
            public Task(CreateDataDelegate method, string path, string namedRange)
            {
                Method = method;
                Path = path;
                NamedRange = namedRange;
            }
            public CreateDataDelegate Method { get; }
            public string Path { get; }
            public string NamedRange { get; }
        }
    }
}
