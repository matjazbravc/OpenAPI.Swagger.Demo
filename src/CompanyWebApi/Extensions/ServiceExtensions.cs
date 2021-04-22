using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
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
        public static void AddApiVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                // Default API Version
                config.DefaultApiVersion = new ApiVersion(1, 1);
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

        public static void AddSwaggerExtension(this IServiceCollection serviceCollection, string apiName)
        {
            serviceCollection.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Title = $"{apiName} v1.0",
                    Version = "v1.0",
                    Description = "*** DEPRECATED WEB API VERSION ***",
                    Contact = new OpenApiContact
                    {
                        Name = "Matjaz Bravc",
                        Email = "matjaz.bravc@gmail.com",
                        Url = new Uri("https://matjazbravc.github.io/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licenced under MIT license",
                        Url = new Uri("http://opensource.org/licenses/mit-license.php")
                    }
                });
                options.SwaggerDoc("v1.1", new OpenApiInfo
                {
                    Title = $"{apiName} v1.1",
                    Version = "v1.1",
                    Description = "DEFAULT WEB API",
                    Contact = new OpenApiContact
                    {
                        Name = "Matjaz Bravc",
                        Email = "matjaz.bravc@gmail.com",
                        Url = new Uri("https://matjazbravc.github.io/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licenced under MIT license",
                        Url = new Uri("http://opensource.org/licenses/mit-license.php")
                    }
                });
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