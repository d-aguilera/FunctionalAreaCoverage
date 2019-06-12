using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using FunctionalAreaCoverage.Helpers;

namespace FunctionalAreaCoverage.Clients
{
    internal class JiraHttpClient : HttpClient
    {
        private const string JiraBaseAddress = "https://jira.devfactory.com";
        private const string RestApiAddress = "/rest/api/2";

        private static string JiraBasicAuthToken
        {
            get => ConfigurationHelper.GetSetting(nameof(JiraBasicAuthToken));
            set => ConfigurationHelper.SaveSetting(nameof(JiraBasicAuthToken), value);
        }

        public JiraHttpClient()
        {
            string token;
            try
            {
                token = JiraBasicAuthToken;
            }
            catch (ConfigurationErrorsException)
            {
                PasswordHelper.WriteLine();
                PasswordHelper.WriteLine("*****************************************");
                PasswordHelper.WriteLine("Jira Auth token not found. Please log in:");
                var username = PasswordHelper.ReadUsername("DevFactory username: ");
                var password = PasswordHelper.ReadPassword();
                PasswordHelper.WriteLine("*****************************************");
                PasswordHelper.WriteLine();

                var cred = new NetworkCredential(username, password);
                token = JiraBasicAuthToken = Base64Helper.Encode($"{cred.UserName}:{cred.Password}");
                TraceHelper.Info("Jira Auth token updated.");
            }
            BaseAddress = new Uri(JiraBaseAddress);
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }

        public HttpResponseMessage Search(string jql, string fields = null, int startAt = 0, int maxResults = 1024)
        {
            return TraceHelper.Run(() => SearchImpl(jql, fields, startAt, maxResults));
        }

        private HttpResponseMessage SearchImpl(string jql, string fields = null, int startAt = 0, int maxResults = 1024)
        {
            dynamic dyn = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(jql))
            {
                throw new ArgumentNullException(nameof(jql));
            }
            dyn.jql = jql;
            if (!string.IsNullOrWhiteSpace(fields))
            {
                dyn.fields = fields;
            }
            if (startAt > 0)
            {
                dyn.startAt = startAt;
            }
            if (maxResults > 0)
            {
                dyn.maxResults = maxResults;
            }

            var dic = (IDictionary<string, object>) dyn;
            var qs = string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var url = $"{RestApiAddress}/search?{qs}";

            return TraceHelper.Run(() => GetImpl(url));
        }

        private HttpResponseMessage GetImpl(string url)
        {
            TraceHelper.WriteLine("Sending request to Jira");
            var response = GetAsync(url).Result;
            TraceHelper.WriteLine($"Response received ({response.StatusCode})");
            return response;
        }
    }
}
