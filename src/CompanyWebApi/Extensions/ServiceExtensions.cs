using CompanyWebApi.Services.Helpers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Linq;
using System.Reflection;
using System;

namespace CompanyWebApi.Extensions
{
    public static class ServiceExtensions
    {
        // Add API Versioning
        // The default version is 1.1
        // And we're going to read the version number from the media type
        // Incoming requests should have a accept header like this: Accept: application/json;v=1.1
        public static void AddVersioning(this IServiceCollection services)
        {
            var configuration = services
                .BuildServiceProvider()
                .GetService<IConfiguration>();

            var defaultApiVersion = ConfigurationHelper.GetDefaultApiVersion(configuration);
            services.AddApiVersioning(config =>
            {
                // Default API Version
                config.DefaultApiVersion = defaultApiVersion;
                // use default version when version is not specified
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
            });
        }

        // More info: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1
        public static void AddCorsPolicy(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("X-Pagination"));
            });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            var configuration = services
                .BuildServiceProvider()
                .GetService<IConfiguration>();

            var swaggerOptions = ConfigurationHelper.GetSwaggerOptions(configuration);
            var swaggerVersions = ConfigurationHelper.GetSwaggerVersions(configuration);

            services.AddSwaggerGen(options =>
            {
                foreach (var swaggerVersion in swaggerVersions.OrderByDescending(v => v.Version))
                {
                    options.SwaggerDoc(swaggerVersion.Version, new OpenApiInfo
                    {
                        Title = swaggerVersion.Title,
                        Version = swaggerVersion.Version,
                        Description = swaggerVersion.Description,
                        Contact = new OpenApiContact
                        {
                            Name = swaggerOptions.ContactName,
                            Email = swaggerOptions.ContactEmail,
                            Url = new Uri(swaggerOptions.ContactUrl)
                        },
                        License = new OpenApiLicense
                        {
                            Name = swaggerOptions.LicenseName,
                            Url = new Uri(swaggerOptions.LicenseUrl)
                        }
                    });
                }
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()

                    }
                });
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor.GetApiVersionModel();
                    // Would mean this action is unversioned and should be included everywhere
                    return actionApiVersionModel.DeclaredApiVersions.Any() ? actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName) : actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });
                var xmlDocFile = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                if (File.Exists(xmlDocFile))
                {
                    options.IncludeXmlComments(xmlDocFile);
                }
                options.DescribeAllParametersInCamelCase();
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
        }
    }
}