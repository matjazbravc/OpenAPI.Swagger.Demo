using CompanyWebApi.Configurations;
using CompanyWebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace CompanyWebApi.Extensions
{
    public static class AppExtensions
    {
        /// <summary>
        /// Adds global exception handling middleware
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ApiExceptionHandlingMiddleware>();

        /// <summary>
        /// Register Swagger and SwaggerUI middleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static void UseSwaggerMiddleware(this IApplicationBuilder app, IConfiguration config)
        {
            var swaggerConfig = config.GetSection(nameof(SwaggerConfig)).Get<SwaggerConfig>();
            app.UseSwagger(options =>
            {
                options.RouteTemplate = $"{swaggerConfig.RoutePrefix}/{{documentName}}/{swaggerConfig.DocsFile}";
            });
            app.UseSwaggerUI(options =>
            {
                options.InjectStylesheet($"/{swaggerConfig.RoutePrefix}/swagger-custom-ui-styles.css");
                options.InjectJavascript($"/{swaggerConfig.RoutePrefix}/swagger-custom-script.js");
            });
        }

        /// <summary>
        /// Register the ReDoc middleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static void UseReDocMiddleware(this IApplicationBuilder app, IConfiguration config)
        {
            var swaggerConfig = config.GetSection(nameof(SwaggerConfig)).Get<SwaggerConfig>();
            app.UseReDoc(sa =>
            {
                sa.DocumentTitle = $"{swaggerConfig.Title} Documentation";
                sa.SpecUrl = $"/{swaggerConfig.RoutePrefix}/V2/{swaggerConfig.DocsFile}";
            });
        }

    }
}
