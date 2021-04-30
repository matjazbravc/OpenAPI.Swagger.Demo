using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Middleware
{
    /// <summary>
    /// Api Exception Handling Middleware
    /// </summary>
    public class ApiExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

        public ApiExceptionHandlingMiddleware(ILogger<ApiExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }

            catch (Exception ex)
            {
                // Handle exception with modifying response with exception details
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handle exception with modifying response
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="ex">Exception</param>
        /// <returns>Task</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            var errorMsg = ex.Message;
            if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
            {
                errorMsg = ex.InnerException.Message;
            }

            _logger.LogError($"{errorMsg}, REQ: {context.Request.Path}");

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = context.Response.StatusCode,
                Instance = context.Request.Path,
                Detail = errorMsg
            };

            var result = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(result).ConfigureAwait(false);
        }
    }
}
