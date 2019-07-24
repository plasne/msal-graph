using System;
using Microsoft.Identity.Client;
using dotenv.net;
using Microsoft.Graph;
using System.Threading.Tasks;

public class Graph
{

    public Graph()
    {
        DotEnv.Config();
    }

    private IConfidentialClientApplication App { get; set; }

    private string ClientId
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("CLIENT_ID");
        }
    }

    private string ClientSecret
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("CLIENT_SECRET");
        }
    }

    private string TenantId
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("TENANT_ID");
        }
    }

    public async Task Poll()
    {
        try
        {

            // start timing how long it takes to get the token
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // get graph with refresh token
            string[] scopes = new string[] { "offline_access https://graph.microsoft.com/.default" };

            // get the token
            AuthenticationResult result = await this.App.AcquireTokenForClient(scopes).ExecuteAsync();

            // report how long it takes to get the token
            watch.Stop();
            Console.WriteLine($"===> elapsed: {watch.ElapsedMilliseconds}");

            // get a graph client
            var graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
                       {
                           requestMessage
                               .Headers
                               .Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.AccessToken);
                           return Task.FromResult(0);
                       }));

            // query for all users
            var users = await graphServiceClient
            .Users
            .Request()
            .Select("displayName")
            .GetAsync();

            // show the users
            foreach (var user in users)
            {
                Console.WriteLine(user.DisplayName);
            }

        }
        catch (MsalServiceException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void Start()
    {

        // build the app
        this.App = ConfidentialClientApplicationBuilder.Create(this.ClientId)
                   .WithTenantId(this.TenantId)
                   .WithClientSecret(this.ClientSecret)
                   .Build();

    }

}