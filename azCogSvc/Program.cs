using System;
using System.IO;
using System.Threading.Tasks;
using azCogSvc.CommandLine;
using Microsoft.Extensions.Configuration;

namespace azcogsvc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Console.WriteLine("Azure Cognitive Services CLI tool\n");

            var appConfig = LoadAppSettings();

            //if (appConfig == null)
            //{
            //    Console.WriteLine("Missing or invalid appsettings.json...exiting");
            //    return;
            //}

            var config = new ParsingConfiguration(args);
            await config.SetupAsync();
            
            //var configValue = appConfig["configValue"];
			//Console.WriteLine("Got config value: [{0}]",configValue);

        }
        static IConfigurationRoot LoadAppSettings()
        {
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var appConfig = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            return appConfig;
        }
    }

}
