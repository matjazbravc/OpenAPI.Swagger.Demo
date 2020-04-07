using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using WebMotions.Fake.Authentication.JwtBearer;

namespace CompanyWebApi.Tests.Services
{
    /// <summary>
    /// Customized WebApplicationFactory
    /// </summary>
    public class WebApiTestFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseContentRoot(".") // Content root directory for web host
                .UseTestServer() // Add TestServer
                .UseEnvironment("Test") // Specify the environment
                .ConfigureTestServices(services =>
                {
                    // Remove the app's ApplicationDbContext registration
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add EntityFramework InMemoryDatabase
                    var serviceProvider = new ServiceCollection()
                      .AddEntityFrameworkInMemoryDatabase()
                      .BuildServiceProvider();

                    // Add ApplicationDbContext using an in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDatabaseTest");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                    // Build the service provider
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the database context (ApplicationDbContext)
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();
                        var logger = scopedServices.GetRequiredService<ILogger<WebApiTestFactory>>();

                        // Ensure the database is created
                        dbContext.Database.EnsureCreated();

                        try
                        {
                            // Seed the database with test data
                            SeedData.Initialize(dbContext);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                        }
                    }

                    // Add fake Jwt authentication
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    }).AddFakeJwtBearer();
                });

            // Call base Configuration
            base.ConfigureWebHost(builder);
        }
    }
}