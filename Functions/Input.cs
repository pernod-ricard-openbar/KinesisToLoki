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
using Microsoft.Extensions.Primitives;

namespace PR.Squid.KinesisToLoki
{
    public class Input
    {
        private LokiClient _lokiClient;
        private string _kinesisAccessKey;

        public Input(LokiClient lokiClient, IConfiguration config) {
            _lokiClient = lokiClient;
            _kinesisAccessKey = config["KinesisAccessKey"];
        }

        private static void AddLokiLabel(Dictionary<string, string> labels, string labelName, string labelValue)
        {
            if (labelValue != null)
            {
                labels.Add(labelName, labelValue);
            }
        }

        // Checks kinesis access key
        private bool CheckAccessKey(string accessKey) {
            return accessKey.Equals(_kinesisAccessKey);
        }

        [FunctionName("Input")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "input")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing a kinesis message");

            // Checking access key
            StringValues accessKey = new StringValues();
            req.Headers.TryGetValue("X-Amz-Kinesis-Access-Key",  out accessKey);
            if (!String.IsNullOrEmpty(_kinesisAccessKey)) {
                if ((accessKey.Count == 0) || ((accessKey.Count > 0) && ! CheckAccessKey(accessKey[0]))) {
                    return new UnauthorizedResult();
                }
            }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            KinesisRequest kinesisRequest = JsonSerializer.Deserialize<KinesisRequest>(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            try {
                // Loop through all records
                foreach (KinesisRecord kinesisRecord in kinesisRequest.Records) {
                    // Decode the base64 content
                    byte[] recordByte = Convert.FromBase64String(kinesisRecord.Data);
                    string record = Encoding.UTF8.GetString(recordByte);
                    //log.LogInformation($"Kinesis content: {record}");

                    // Build the Loki entry and send it
                    Dictionary<string, string> labels = new Dictionary<string, string>();
                    AddLokiLabel(labels, "project", "API Management Cloudfront");
                    LokiLogEntry lokiLogEntry = new LokiLogEntry(
                        labels,
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + "000000",
                        record);
                    await _lokiClient.SendLogsAsync(lokiLogEntry);
                    
                }

                // Send back the result to Kinesis
                KinesisResponseSuccess kinesisResponseSuccess = new KinesisResponseSuccess(kinesisRequest.RequestId, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                return new OkObjectResult(kinesisResponseSuccess);

            }
            catch (Exception e) {
                // Send back the error to Kinesis
                KinesisResponseError kinesisResponseError = new KinesisResponseError(kinesisRequest.RequestId, DateTimeOffset.Now.ToUnixTimeMilliseconds(), e.Message);
                return new BadRequestObjectResult(kinesisResponseError);
            }
        }
    }
}
