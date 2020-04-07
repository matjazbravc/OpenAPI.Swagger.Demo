using CompanyWebApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Middleware
{
    /// <summary>
    /// Global Exception Handling Middleware
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Trigger the next middleware component 
                await _next(httpContext).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Request failed because of exception
                var errorMsg = $"Request Path: {httpContext.Request.Path}, {ex.Message}";
                // Write exception into the log file
                Log.Error(errorMsg);
                // Handle exception with modifying response with exception details
                await HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handle exception with modifying response
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <param name="ex">Exception</param>
        /// <returns>Task</returns>
        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorMsg = ex.Message;
            if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
            {
                errorMsg = ex.InnerException.Message;
            }
            return httpContext.Response.WriteAsync(new ApiError(httpContext.Response.StatusCode, errorMsg).ToString());
        }
    }
}
