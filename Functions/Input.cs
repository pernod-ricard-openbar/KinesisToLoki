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
            req.Headers.TryGetValue("X-Amz-Firehose-Access-Key",  out accessKey);
            if (!String.IsNullOrEmpty(_kinesisAccessKey)) {
                if ((accessKey.Count == 0) || ((accessKey.Count > 0) && ! CheckAccessKey(accessKey[0]))) {
                    return new UnauthorizedResult();
                }
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            KinesisRequest kinesisRequest = JsonSerializer.Deserialize<KinesisRequest>(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            try {
                // Loop through all records send by Kinesis
                foreach (KinesisRecord kinesisRecord in kinesisRequest.Records) {
                    // Decode the base64 content
                    byte[] recordByte = Convert.FromBase64String(kinesisRecord.Data);
                    string record = Encoding.UTF8.GetString(recordByte);
                    
                    // Send to loki using the loki client
                    await _lokiClient.SendLogsAsync(record);  
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
