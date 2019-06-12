using System;
using System.Collections.Generic;
using System.Linq;
using FunctionalAreaCoverage.Clients;
using FunctionalAreaCoverage.Entities;
using Google.Apis.Sheets.v4.Data;

namespace FunctionalAreaCoverage.Helpers
{
    internal static class SheetsHelper
    {
        private static string SpreadsheetId => ConfigurationHelper.GetSetting("SpreadsheetId");

        public static void UpdateRange(string rangeName, IEnumerable<IEnumerable<string>> tabularData)
        {
            TraceHelper.Run(() => UpdateRangeImpl(rangeName, tabularData));
        }

        private static void UpdateRangeImpl(string rangeName, IEnumerable<IEnumerable<string>> tabularData)
        {
            using (var client = new SheetsClient())
            {
                var request = client.Spreadsheets.Get(SpreadsheetId);
                var spreadsheet = request.Execute();
                var range = spreadsheet.NamedRanges.Single(r => r.Name == rangeName).Range;
                if (range.EndRowIndex - range.StartRowIndex < 1)
                {
                    throw new InvalidOperationException($"{rangeName} range is too short");
                }

                var array = tabularData.ToArray();
                var body = new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request>
                    {
                        CreateDeleteRequest(range),
                        CreateInsertRequest(range, array),
                        CreateCopyPasteRequest(range, array),
                        CreateUpdateRequest(range, array)
                    }
                };
                var updateRequest = client.Spreadsheets.BatchUpdate(body, SpreadsheetId);
                updateRequest.Execute();
            }
        }

        public static void AddFillerData()
        {
            TraceHelper.Run(() => AddFillerDataImpl());
        }
        
        private static void AddFillerDataImpl()
        {
            const string rangeName = "FAE2E_DATA";
            var statuses = Enum.GetNames(typeof(EndToEndStatus))
                .Where(s => s != $"{EndToEndStatus.Unknown}")
                .Select(CamelHelper.ToProperName)
                .ToArray();
            var tabularData = Enumerable.Range(1, statuses.Length)
                .Select(i => new[] {"_", null, null, null, $"{i:00}-{statuses[i - 1]}", null})
                .ToArray();

            using (var client = new SheetsClient())
            {
                var request = client.Spreadsheets.Get(SpreadsheetId);
                var spreadsheet = request.Execute();
                var range = spreadsheet.NamedRanges.Single(r => r.Name == rangeName).Range;
                range.StartRowIndex++;
                if (range.EndRowIndex - range.StartRowIndex < 1)
                {
                    throw new InvalidOperationException($"{rangeName} range is too short");
                }

                var body = new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request>
                    {
                        CreateInsertRequest(range, tabularData),
                        CreateCopyPasteRequest(range, tabularData),
                        CreateUpdateRequest(range, tabularData)
                    }
                };
                var updateRequest = client.Spreadsheets.BatchUpdate(body, SpreadsheetId);
                updateRequest.Execute();
            }
        }

        private static Request CreateDeleteRequest(GridRange range) => new Request
        {
            DeleteDimension = new DeleteDimensionRequest
            {
                Range = new DimensionRange
                {
                    SheetId = range.SheetId,
                    Dimension = "ROWS",
                    StartIndex = range.StartRowIndex + 1,
                    EndIndex = range.EndRowIndex - 1
                }
            }
        };

        private static Request CreateInsertRequest(GridRange range, IReadOnlyCollection<IEnumerable<string>> array) => new Request
        {
            InsertDimension = new InsertDimensionRequest
            {
                Range = new DimensionRange
                {
                    SheetId = range.SheetId,
                    Dimension = "ROWS",
                    StartIndex = range.StartRowIndex + 1,
                    EndIndex = range.StartRowIndex + array.Count - 1
                }
            }
        };

        private static Request CreateCopyPasteRequest(GridRange range, IReadOnlyCollection<IEnumerable<string>> array) => new Request
        {
            CopyPaste = new CopyPasteRequest
            {
                Source = new GridRange
                {
                    SheetId = range.SheetId,
                    StartRowIndex = range.StartRowIndex,
                    StartColumnIndex = range.StartColumnIndex,
                    EndRowIndex = range.StartRowIndex + 1,
                    EndColumnIndex = range.EndColumnIndex
                },
                Destination = new GridRange
                {
                    SheetId = range.SheetId,
                    StartRowIndex = range.StartRowIndex + 1,
                    StartColumnIndex = range.StartColumnIndex,
                    EndRowIndex = range.StartRowIndex + array.Count - 1,
                    EndColumnIndex = range.EndColumnIndex
                }
            }
        };

        private static Request CreateUpdateRequest(GridRange range, IEnumerable<IEnumerable<string>> array) => new Request
        {
            UpdateCells = new UpdateCellsRequest
            {
                Fields = "*",
                Rows = array.Select(StringRowData).ToList(),
                Start = new GridCoordinate
                {
                    SheetId = range.SheetId,
                    RowIndex = range.StartRowIndex,
                    ColumnIndex = range.StartColumnIndex
                }
            }
        };

        private static RowData StringRowData(IEnumerable<string> values)
        {
            return new RowData
            {
                Values = values.Select(StringCellData).ToList()
            };
        }

        private static CellData StringCellData(string value)
        {
            return new CellData
            {
                UserEnteredValue = new ExtendedValue
                {
                    StringValue = value
                }
            };
        }
    }
}
