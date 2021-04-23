using CompanyWebApi.Middleware;
using CompanyWebApi.Services.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

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

        public static void UseSwagger(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            var swaggerVersions = ConfigurationHelper.GetSwaggerVersions(configuration);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            SwaggerBuilderExtensions.UseSwagger(app);

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio
            app.UseSwaggerUI(config =>
            {
                foreach (var swaggerVersion in swaggerVersions.OrderByDescending(v => v.Version))
                {
                    config.SwaggerEndpoint(swaggerVersion.UIEndpoint, swaggerVersion.Version);
                }
            });
        }
    }
}
