using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paybills.API.Data;
using Paybills.API.Infrastructure.Helpers;
// using Serilog;
// using Serilog.Events;
// using Serilog.Sinks.Elasticsearch;

namespace Paybills.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY");
            if (string.IsNullOrWhiteSpace(tokenKey))
            {
                throw new InvalidOperationException("Environment variable TOKEN_KEY is not set. Application cannot start.");
            }
            ConfigureLogging();

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                    await context.Database.MigrateAsync();
                    // await Seed.SeedUsers(context);
                }
                catch (Exception e)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "An error ocurred during migration");
                }
            }

            await host.RunAsync();
        }

        private static void ConfigureLogging()
        {
            var appRootDirectory = Directory.GetCurrentDirectory();
            var dotEnvFilePath = Path.Combine(appRootDirectory, ".env");
            DotEnv.Load(dotEnvFilePath);

            // var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                // .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .Build();



            // Log.Logger = new LoggerConfiguration()
            // //     .Enrich.FromLogContext()
            //     .WriteTo.Debug()
            //     .WriteTo.Console()

            //     // .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200")) {
            //     //     IndexFormat = $"paybills-api-{DateTime.UtcNow:yyyy-MM}",
            //     //     AutoRegisterTemplate = true
            //     // })

            //     // .ReadFrom.Configuration(configuration)
            //     .CreateLogger();
        }

        // private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        // {
        //     return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:uri"]))
        //     {
        //         AutoRegisterTemplate = true,
        //         IndexFormat = $"centralizador-logs-{DateTime.UtcNow:yyyy-MM}"
        //     };
        // }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
