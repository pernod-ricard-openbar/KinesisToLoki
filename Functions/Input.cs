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
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PR.Squid.FirehoseToLoki
{
    public class Input
    {
        private LokiClient _lokiClient;

        public Input(LokiClient lokiClient) {
            _lokiClient = lokiClient;      
        }

        private static void AddLokiLabel(Dictionary<string, string> labels, string labelName, string labelValue)
        {
            if (labelValue != null)
            {
                labels.Add(labelName, labelValue);
            }
        }

        [FunctionName("Input")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("Processing a firehose message");

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            FirehoseRequest firehoseRequest = JsonSerializer.Deserialize<FirehoseRequest>(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            try {
                // Loop through all records
                foreach (FirehoseRecord firehoseRecord in firehoseRequest.Records) {
                    // Decode the base64 content
                    byte[] recordByte = Convert.FromBase64String(firehoseRecord.Data);
                    string record = Encoding.UTF8.GetString(recordByte);
                    //log.LogInformation($"Firehose content: {record}");

                    // Build the Loki entry and send it
                    Dictionary<string, string> labels = new Dictionary<string, string>();
                    AddLokiLabel(labels, "project", "API Management Cloudfront");
                    LokiLogEntry lokiLogEntry = new LokiLogEntry(
                        labels,
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + "000000",
                        record);
                    await _lokiClient.SendLogsAsync(lokiLogEntry);
                    
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
