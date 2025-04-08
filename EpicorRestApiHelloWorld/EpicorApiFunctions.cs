using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

class EpicorApiFunctions
{
    // THESE FUNCTIONS ARE "GENERIC" TO INTERFACE WITH THE EPICOR REST API
    // DOCS: https://ee-erp11-test.excoeng.com/ERP11-TST/api/help/v2/index.html
    // IN OTHER PROJECTS THE EPICOR OBJECTS RELY ON THESE TO GET/PUSH/PATCH/DELETE DATA FROM EPICOR
    // IT IS UP TO YOU IF YOU WANT TO PUT THIS LOGIC ON YOUR OBJECTS OR KEEP SEPARATE, THIS IS JUST AN EXAMPLE.

    private static HttpClient GetAuthenticatedClientFromEnviromentVariables(string uri_address)
    {
        string userName = Environment.GetEnvironmentVariable("EPICOR_EXCO_API_READONLY_SERVER_USER");
        string password = Environment.GetEnvironmentVariable("EPICOR_EXCO_API_READONLY_SERVER_PASSWORD");
        string currentApiKey = Environment.GetEnvironmentVariable("EPICOR_EXCO_API_KEY");

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(currentApiKey))
        {
            string message = "One or more required environment variables are missing:\n" +
                             $"{(string.IsNullOrWhiteSpace(userName) ? "- EPICOR_EXCO_API_READONLY_SERVER_USER\n" : "")}" +
                             $"{(string.IsNullOrWhiteSpace(password) ? "- EPICOR_EXCO_API_READONLY_SERVER_PASSWORD\n" : "")}" +
                             $"{(string.IsNullOrWhiteSpace(currentApiKey) ? "- EPICOR_EXCO_API_KEY\n" : "")}";
            throw new InvalidOperationException("Required Epicor API environment variables are missing.");
        }

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(uri_address)
        };

        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        httpClient.DefaultRequestHeaders.Add("x-api-key", currentApiKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

        return httpClient;
    }

    private static string GetCurrentServer(bool useTestServer = true)
    {
        string CURRENT_COMPANY = "200";
        string PRD_SERVER = $"https://ee-erp11-app.excoeng.com/ERP11-PRD/api/v2/odata/{CURRENT_COMPANY}/";
        string TEST_SERVER = $"https://ee-erp11-test.excoeng.com/ERP11-TST/api/v2/odata/{CURRENT_COMPANY}/";

        string currentServer = null;
        if (useTestServer)
        {
            currentServer = TEST_SERVER;
        }
        else if (!useTestServer)
        {
            currentServer = PRD_SERVER;
        }
        return currentServer;
    }

    public static string HitEpicorWithAGet(string epicorService, string parameterString, string argumentString = "", bool useTestServer = true)
    {
        string currentServer = GetCurrentServer(useTestServer);
        string baseUri = $"";
        if (argumentString != "")
        {
            baseUri = $"{currentServer}{epicorService}({argumentString})?{parameterString}";
        }
        else
        {
            baseUri = $"{currentServer}{epicorService}?{parameterString}";
        }
        UriBuilder uriBuilder = new UriBuilder(baseUri);
        HttpClient httpClient = GetAuthenticatedClientFromEnviromentVariables(uriBuilder.ToString());
        string responseBody = "";
        try
        {
            if (useTestServer) // test server usually no ssl
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            }

            HttpResponseMessage response = httpClient.GetAsync("").GetAwaiter().GetResult(); // GET no arguments no parameters no body
            responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return responseBody;
        }
        catch //(HttpRequestException e)
        {
            string issue = $"There was an issue during GET to service {epicorService}";
            Debug.Write($"ISSUE, {issue}");
            Debug.Write($"URI, {baseUri}");
            Debug.Write($"RESPONSEBODY, {responseBody}");
            Debug.Write($"\n");
            return null;
        }
    }
}