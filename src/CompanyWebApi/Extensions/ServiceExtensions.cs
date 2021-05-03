using CompanyWebApi.Services.Swagger;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CompanyWebApi.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds service API versioning
        /// </summary>
        /// <param name="services"></param>
        public static void AddAndConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // Specify the default API Version
                options.DefaultApiVersion = new ApiVersion(2, 0);
                // Use default version when version is not specified
                options.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                options.ReportApiVersions = true;
                // Adds a convention to let Swagger understand the different API versions
                options.Conventions.Add(new VersionByNamespaceConvention());
            });

            services.AddVersionedApiExplorer(options =>
            {
                // Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // Note: the specified format code will format the version as "'V'major[.minor][-status]"
                options.GroupNameFormat = "'V'VVV";

                // Note: this option is only necessary when versioning by url segment.
                // The SubstitutionFormat can also be used to control the format of the API version in route templates.
                options.SubstituteApiVersionInUrl = true;
            });
        }

        /// <summary>
        /// Adds cross-origin resource sharing services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="policyName"></param>
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

        /// <summary>
        /// Adds Swagger support
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddSwaggerMiddleware(this IServiceCollection services)
        {
            // Configure Swagger Options
            services.AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUiOptions>();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

            // Register the Swagger generator
            services.AddSwaggerGen();
        }
    }
}