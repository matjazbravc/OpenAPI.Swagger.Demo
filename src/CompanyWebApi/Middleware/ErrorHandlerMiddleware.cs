using CompanyWebApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Middleware
{
    /// <summary>
    /// Global Error Handling Middleware
    /// </summary>
    public class ErrorHandlerMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ApiLoggingMiddleware>();
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

            var apiError = new ApiError(context.Response.StatusCode, errorMsg, ex.GetType().ToString(), context.Request.Path);
            var result = JsonConvert.SerializeObject(apiError);

            _logger.LogError($"{apiError.Message}: {apiError.StatusDescription}, REQ: {apiError.RequestPath}");

            await context.Response.WriteAsync(result).ConfigureAwait(false);
        }
    }
}
