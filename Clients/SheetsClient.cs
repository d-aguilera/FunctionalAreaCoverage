using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace FunctionalAreaCoverage.Clients
{
    internal class SheetsClient : SheetsService
    {
        public SheetsClient() : base(new Initializer
        {
            HttpClientInitializer = GetCredential(),
            ApplicationName = "Functional Area Coverage",
        })
        {
        }

        private static UserCredential GetCredential()
        {
            using (var stream = new FileStream("Properties/credentials.base64", FileMode.Open, FileAccess.Read))
            using (var decoded = new CryptoStream(stream, new FromBase64Transform(), CryptoStreamMode.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(decoded).Secrets,
                    new[] {Scope.Spreadsheets},
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)).Result;
            }
        }
    }
}
