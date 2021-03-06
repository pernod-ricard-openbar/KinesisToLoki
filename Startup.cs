using System.IO;
using System.Net.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(PR.Squid.KinesisToLoki.Startup))]

namespace PR.Squid.KinesisToLoki
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
             builder.Services.AddHttpClient();
             builder.Services.AddSingleton<LokiClient>();
             builder.Services.AddSingleton<CloudFrontLogParser>();
        }
        
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "local.settings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"{context.EnvironmentName}.settings.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    }
}