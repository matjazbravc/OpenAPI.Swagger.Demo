using CompanyWebApi.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CompanyWebApi.Extensions
{
    public static class AppExtensions
    {
        public static IApplicationBuilder UseApiLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLogging>();
        }

        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1.1/swagger.json", "v1.1");
                config.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0");
            });
        }
    }
}
