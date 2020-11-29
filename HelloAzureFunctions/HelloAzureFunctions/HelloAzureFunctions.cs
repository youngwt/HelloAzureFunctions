using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HelloAzureFunctions
{
    public static class HelloAzureFunctions
    {
        [FunctionName("HelloAzureFunctions")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("My first Azure functions end point has been called!");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(requestBody);

            string responseMessage = myDeserializedClass == null ?
                "Did not serialise the request correctly. No zen for you today" :
                myDeserializedClass.zen;

            return new OkObjectResult(responseMessage);
        }
    }
}


