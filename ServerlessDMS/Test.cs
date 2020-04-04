using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ServerlessDMS.Authentication;
using System.Net.Http;

namespace ServerlessDMS
{
    public static class Test
    {
        [FunctionName("Test")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
                                       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                       .AddEnvironmentVariables()
                                       .Build();

            var authenticationService = new AuthenticationService(config["tenantId"], config["clientId"], config["clientSecret"]);
            var token = await authenticationService.GetAccessTokenAsync();
            var rootFolder = $"{config["graphEndpoint"]}sites/{config["siteId"]}/drive/items/root/children";
            var itemPath = $"{config["graphEndpoint"]}sites/{config["siteId"]}/drive/items/01SD22A7FDNSY2DDSSPZEL22OCHDZCL5DM";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.GetAsync(itemPath);
            var test1 = await response.Content.ReadAsStringAsync();

            throw new NotImplementedException();
        }
    }
}
