using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System;
using Microsoft.Extensions.Logging;

namespace CompanyWebApi
{
    //public class ProgramOld
    //{
    //    public static void Main(string[] args)
    //    {
    //        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    //        var appSettings = env == null ? "appsettings.json" : $"appsettings.{env}.json";

    //        //Read Configuration from appSettings
    //        var config = new ConfigurationBuilder()
    //            .AddJsonFile(appSettings)
    //            .AddEnvironmentVariables()
    //            .Build();

    //        //Initialize Logger
    //        Log.Logger = new LoggerConfiguration()
    //            .ReadFrom.Configuration(config)
    //            .CreateLogger();

    //        try
    //        {
    //            Log.Information("Starting application");
    //            CreateHostBuilder(args)
    //                .Build()
    //                .Run();
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error(ex, "The application failed to start correctly");
    //            throw;
    //        }
    //        finally
    //        {
    //            Log.Information("Shutting down application");
    //            Log.CloseAndFlush();
    //        }
    //    }

    //    public static IHostBuilder CreateHostBuilder(string[] args) =>
    //        Host.CreateDefaultBuilder(args)
    //            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    //                .ReadFrom.Configuration(hostingContext.Configuration)
    //                .Enrich.FromLogContext())
    //            .ConfigureWebHostDefaults(webBuilder =>
    //            {
    //                webBuilder.UseStartup<Startup>()
    //                    .UseContentRoot(Directory.GetCurrentDirectory())
    //                    .UseKestrel();
    //            });

    //}

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Configure Serilog
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext())
                // Set the content root to be the current directory
                .UseContentRoot(Directory.GetCurrentDirectory())
                // Disable the dependency injection scope validation feature
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseKestrel();
                })
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var env = builderContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((builderContext, logging) =>
                {
                    // Clear default logging providers
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddSerilog();
                });
    }
}