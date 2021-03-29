using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PR.Squid.FirehoseToLoki
{
    // Basic class to send logs to Loki
    public class LokiClient {

        private HttpClient _httpClient;
        private string _endpoint;
        private string _username;
        private string _password;

        // Constructor
        public LokiClient(HttpClient httpClient, IConfiguration config) {
            _httpClient = httpClient;
            _endpoint = config["LokiEndPoint"];
            _username = config["LokiUsername"];
            _password = config["LokiPassword"];

            ConfigureHttpClient();
        }

        // Configure HttpClient with authorization headers
        private void ConfigureHttpClient() {
            // Adding basic authentication if specified
            if (!String.IsNullOrEmpty(_username) && !String.IsNullOrEmpty(_password))
            {
                var creds = Encoding.ASCII.GetBytes($"{_username}:{_password}");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(creds)); 
            }
            
        }

        // Sends to Loki
        public async Task SendLogsAsync(LokiLogEntry lokiLogEntry) {
            string logContent = JsonSerializer.Serialize<LokiLogEntry>(lokiLogEntry, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            StringContent stringContent = new StringContent(logContent, Encoding.UTF8, "application/json");
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            await _httpClient.PostAsync(_endpoint, stringContent);
        }
    }
}