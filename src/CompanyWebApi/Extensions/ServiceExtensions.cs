using CompanyWebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Abstractions;
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
        // More info: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-5.0
        public static void AddCorsPolicy(this IServiceCollection serviceCollection, string corsPolicyName)
        {
            serviceCollection.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName,
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public static void ConfigureSwagger(this IServiceCollection serviceCollection, string apiName, bool includeXmlDocumentation = true)
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
                            new string[] {}

                    }
                });
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor.GetApiVersionModel();
                    // Would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    return actionApiVersionModel.DeclaredApiVersions.Any() ? actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName) : actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });
                if (includeXmlDocumentation)
                {
                    var xmlDocFile = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                    if (File.Exists(xmlDocFile))
                    {
                        options.IncludeXmlComments(xmlDocFile);
                    }
                }
                options.DescribeAllParametersInCamelCase();
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
        }

        public static IApplicationBuilder UseApiLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLoggingMiddleware>();
        }

        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}