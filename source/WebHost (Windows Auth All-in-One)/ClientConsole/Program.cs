using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var resultToken = RequestTokenWindowsAuth();

            ShowResponse(resultToken);

            Console.WriteLine(Environment.NewLine);

            "===============================================".ConsoleRed();
            "CONNECTING TO IDENTITY SERVER TO VALIDATE TOKEN".ConsoleRed();
            "===============================================".ConsoleRed();

            Console.WriteLine(Environment.NewLine);

            var clientForAuth = new TokenClient(Constants.TokenEndpoint);

            var additionalvalues = new Dictionary<string, string>()
            {
                { "client_id", "client" },
                { "client_secret", "secret" },
                { "win_token", resultToken.AccessToken }
            };

            var authResult = clientForAuth.RequestCustomGrantAsync("windows", "api write", additionalvalues).Result;

            ShowResponse(authResult);

            CallService(authResult.AccessToken);

            Console.ReadLine();
        }

        static TokenResponse RequestTokenWindowsAuth()
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };
            
            var client = new TokenClient(
                Constants.WindowsTokenEndpoint,
                handler);

            return client.RequestCustomGrantAsync("windows").Result;
        }

        private static void ShowResponse(TokenResponse response)
        {
            if (!response.IsError)
            {
                "Token response:".ConsoleGreen();
                Console.WriteLine(response.Json);

                if (response.AccessToken.Contains("."))
                {
                    "\nAccess Token (decoded):".ConsoleGreen();

                    var parts = response.AccessToken.Split('.');
                    var header = parts[0];
                    var claims = parts[1];


                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
                }
            }
            else
            {
                if (response.IsHttpError)
                {
                    "HTTP error: ".ConsoleGreen();
                    Console.WriteLine(response.HttpErrorStatusCode);
                    "HTTP error reason: ".ConsoleGreen();
                    Console.WriteLine(response.HttpErrorReason);
                }
                else
                {
                    "Protocol error response:".ConsoleGreen();
                    Console.WriteLine(response.Json);
                }
            }
        }

        static void CallService(string token)
        {
            var baseAddress = Constants.AspNetWebApiSampleApi;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            var response = client.GetStringAsync("identity").Result;

            "\n\nService claims:".ConsoleGreen();
            Console.WriteLine(JArray.Parse(response));
        }
    }

    public static class Constants
    {
        public const string BaseAddress = "https://localhost:44333";

        public const string WindowsTokenEndpoint = BaseAddress + "/windows/token";
        public const string AuthorizeEndpoint = BaseAddress + "/connect/authorize";
        public const string LogoutEndpoint = BaseAddress + "/connect/endsession";
        public const string TokenEndpoint = BaseAddress + "/identity/connect/token";
        public const string UserInfoEndpoint = BaseAddress + "/connect/userinfo";
        public const string IdentityTokenValidationEndpoint = BaseAddress + "/connect/identitytokenvalidation";
        public const string TokenRevocationEndpoint = BaseAddress + "/connect/revocation";

        public const string AspNetWebApiSampleApi = "http://localhost:2727/";
        public const string AspNetWebApiSampleApiUsingPoP = "http://localhost:46613/";
    }
}
