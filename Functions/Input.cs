using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace PR.Squid.FirehoseToLoki
{
    public static class Input
    {
        [FunctionName("Input")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing a firehose message");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            FirehoseRequest firehoseRequest = JsonSerializer.Deserialize<FirehoseRequest>(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            try {
                // Loop through all records
                foreach (FirehoseRecord firehoseRecord in firehoseRequest.Records) {
                    // Decode the base64 content
                    byte[] recordByte = Convert.FromBase64String(firehoseRecord.Data);
                    string record = Encoding.UTF8.GetString(recordByte);
                }

                // Send back the result to Firehose
                FirehoseResponseSuccess firehoseResponseSuccess = new FirehoseResponseSuccess(firehoseRequest.RequestId, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                return new OkObjectResult(firehoseResponseSuccess);

            }
            catch (Exception e) {
                // Send back the error to Firehose
                FirehoseResponseError firehoseResponseError = new FirehoseResponseError(firehoseRequest.RequestId, DateTimeOffset.Now.ToUnixTimeMilliseconds(), e.Message);
                return new BadRequestObjectResult(firehoseResponseError);
            }
        }
    }
}
