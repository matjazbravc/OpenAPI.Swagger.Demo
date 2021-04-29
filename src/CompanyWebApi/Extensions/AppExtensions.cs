using CompanyWebApi.Configurations;
using CompanyWebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace CompanyWebApi.Extensions
{
    public static class AppExtensions
    {
        /// <summary>
        /// Adds API logging middleware
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLoggingMiddleware>();
        }

        /// <summary>
        /// Adds error handling middleware
        /// </summary>
        /// <param name="app"></param>
        public static void UseGlobalErrorHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        /// <summary>
        /// Register Swagger and SwaggerUI middleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
        public static void UseSwaggerMiddleware(this IApplicationBuilder app, IConfiguration config)
        {
            var swaggerConfig = config.GetSection("SwaggerConfig").Get<SwaggerConfig>();
            app.UseSwagger(options =>
            {
                options.RouteTemplate = swaggerConfig.RouteTemplate;
            });
            app.UseSwaggerUI();
        }
    }
}
