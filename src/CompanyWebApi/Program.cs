using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var host = CreateHostBuilder(args).Build();
			CreateDbIfNotExists(host);
			host.Run();
		}

        private static void CreateDbIfNotExists(IHost host)
        {
	        using var scope = host.Services.CreateScope();
	        var services = scope.ServiceProvider;
			try
	        {
		        // Seed test data
		        var dbInitializer = services.GetRequiredService<DbInitializer>();
		        dbInitializer.Initialize();
	        }
	        catch (Exception ex)
	        {
		        var logger = services.GetRequiredService<ILogger<Program>>();
		        logger.LogError(ex, "An error occurred creating the DB.");
	        }
        }

		public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Configure Serilog
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
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
                .ConfigureLogging(logging =>
                {
                    // Clear default logging providers
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddSerilog();
                });
    }
}