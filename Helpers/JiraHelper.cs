using FunctionalAreaCoverage.Clients;
using FunctionalAreaCoverage.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalAreaCoverage.Helpers
{
    internal static class JiraHelper
    {
        public static IEnumerable<FunctionalArea> GetFunctionalAreas()
        {
            return TraceHelper.Run(() => GetFunctionalAreasImpl());
        }

        private static IEnumerable<FunctionalArea> GetFunctionalAreasImpl()
        {
            var faJson = CacheHelper.Get(CacheHelper.FunctionalAreasKey, GetFunctionalAreasResponse);
            var eteJson = CacheHelper.Get(CacheHelper.EndToEndsKey, GetEndToEndsResponse);
            return ParseFunctionalAreasResponse(faJson, eteJson);
        }

        private static string GetFunctionalAreasResponse()
        {
            return TraceHelper.Run(() => GetFunctionalAreasResponseImpl());
        }

        private static string GetFunctionalAreasResponseImpl()
        {
            return GetResponseImpl("type='Functional Area'", "summary,status,issuelinks");
        }

        private static string GetEndToEndsResponse()
        {
            return TraceHelper.Run(() => GetEndToEndsResponseImpl());
        }

        private static string GetEndToEndsResponseImpl()
        {
            return GetResponseImpl("type='End-to-end Test'", $"summary,status,{ConfigurationHelper.TestSuiteCategoryFieldName}");
        }

        private static string GetResponseImpl(string projectJql, string fields)
        {
            using (var client = new JiraHttpClient())
            {
                var jql = $"project='{ConfigurationHelper.ProjectName}' AND ({projectJql})";
                var response = client.Search(jql, fields);
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private static IEnumerable<FunctionalArea> ParseFunctionalAreasResponse(string faJson, string eteJson)
        {
            return TraceHelper.Run(() => ParseFunctionalAreasResponseImpl(faJson, eteJson));
        }

        private static IEnumerable<FunctionalArea> ParseFunctionalAreasResponseImpl(string faJson, string eteJson)
        {
            var testSuiteCategoryFieldName = ConfigurationHelper.TestSuiteCategoryFieldName;
            dynamic eteResult = JsonConvert.DeserializeObject(eteJson);
            var eteDic = ((IEnumerable<dynamic>) eteResult.issues)
                .Select(x => new EndToEnd(x.key.Value)
                {
                    Summary = x.fields.summary.Value,
                    Status = x.fields.status.name.Value,
                    TestSuiteCategory = ValueOrDefault(x.fields[testSuiteCategoryFieldName])
                })
                .ToDictionary(ete => ete.Key);

            dynamic faResult = JsonConvert.DeserializeObject(faJson);

            var total = (int) faResult.total;
            TraceHelper.Info($"{total} records");

            var list = new List<FunctionalArea>(total);
            foreach (var issue in faResult.issues)
            {
                var coveredBy = new List<EndToEnd>();
                var fa = new FunctionalArea(issue.key.Value)
                {
                    Summary = issue.fields.summary.Value,
                    Status = issue.fields.status.name.Value,
                    CoveredBy = coveredBy
                };
                foreach (var link in issue.fields.issuelinks)
                {
                    if (link.type.name != "Functional Area Coverage")
                    {
                        continue;
                    }

                    var outwardIssue = link.outwardIssue;
                    coveredBy.Add(eteDic[outwardIssue.key.Value]);
                }

                list.Add(fa);

                TraceHelper.Info($"Parsed {fa.Key} (covered by {fa.CoveredBy.Count()} tests)");
            }

            return list;
        }

        private static string ValueOrDefault(dynamic jToken)
        {
            return jToken.Type == Newtonsoft.Json.Linq.JTokenType.Null
                ? null
                : jToken.value.Value;
        }
    }
}
