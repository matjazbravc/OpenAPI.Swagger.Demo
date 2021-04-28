using CompanyWebApi.Configurations;
using CompanyWebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CompanyWebApi.Extensions
{
    public static class AppExtensions
    {
        public static IApplicationBuilder UseApiLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLoggingMiddleware>();
        }

        public static void UseGlobalErrorHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

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
