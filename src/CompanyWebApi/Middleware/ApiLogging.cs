using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Middleware
{
    /// <summary>
    /// Global Request/response logging middleware
    /// </summary>
    public class ApiLogging : IMiddleware
    {
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public ApiLogging(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ApiLogging>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Get host IP address
            var remoteIpAddress = context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
            if (remoteIpAddress != null)
            {
                var remoteIp = remoteIpAddress.ToString();
                var ipAddress = "127.0.0.1";
                if (!string.IsNullOrEmpty(remoteIp))
                {
                    ipAddress = remoteIp == "::1" ? "127.0.0.1" : remoteIp;
                }

                var stopWatch = Stopwatch.StartNew();

                // Ensure that request body can be read multiple-times
                context.Request.EnableBuffering();

                // Create new memory stream
                await using var requestBodyStream = _recyclableMemoryStreamManager.GetStream();

                // Copy request body stream to new memory stream
                await context.Request.Body.CopyToAsync(requestBodyStream).ConfigureAwait(false);

                // Read request body to string
                var requestBodyString = await new StreamReader(requestBodyStream).ReadToEndAsync().ConfigureAwait(false);

                // Reset request body stream position at begin
                context.Request.Body.Seek(0, SeekOrigin.Begin);

                var request = context.Request;
                var requestQueryString = request.QueryString.ToString();

                var response = context.Response;

                // Save original response body stream
                var originalBodyStream = response.Body;

                // Create new memory stream
                await using var responseBodyStream = _recyclableMemoryStreamManager.GetStream();
                response.Body = responseBodyStream;

                // Trigger the next middleware component 
                await next(context).ConfigureAwait(false);

                stopWatch.Stop();

                // Read response body to string
                response.Body.Seek(0, SeekOrigin.Begin);

                // Read response body stream to string
                var responseBodyString = await new StreamReader(response.Body).ReadToEndAsync().ConfigureAwait(false);

                // Reset request body stream position at begin
                response.Body.Seek(0, SeekOrigin.Begin);

                LogInformations(request, response, stopWatch.ElapsedMilliseconds, 
                    ipAddress, requestQueryString, requestBodyString, responseBodyString);

                // Restore original response body
                await responseBodyStream.CopyToAsync(originalBodyStream).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Log informations to the file
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="requestElapsedMilliseconds"></param>
        /// <param name="ipAddress"></param>
        /// <param name="requestQueryString"></param>
        /// <param name="requestBodyString"></param>
        /// <param name="responseBodyString"></param>
        private void LogInformations(HttpRequest request, HttpResponse response, long requestElapsedMilliseconds, 
            string ipAddress, string requestQueryString, string requestBodyString, string responseBodyString)
        {
            // Trim response body if neccessary
            if (responseBodyString.Length > 1024)
            {
                responseBodyString = $"(Truncated to 1024 chars) {responseBodyString.Substring(0, 1024)}";
            }

            // Trim request query string if neccessary
            if (requestQueryString.Length > 1024)
            {
                requestQueryString = $"(Truncated to 1024 chars) {requestQueryString.Substring(0, 1024)}";
            }

            // Filter out Authentication requests
            if (request.Path.ToString().Contains("authenticate", StringComparison.CurrentCultureIgnoreCase))
            {
                requestBodyString = "(Request logging disabled for Authentication)";
                responseBodyString = "(Response logging disabled for Authentication)";
            }

            // Create log information
            var sb = new StringBuilder();
            sb.Append($"IP: {ipAddress} ");
            sb.Append($"ElapsedMillis: {requestElapsedMilliseconds} ");
            sb.Append($"Status: {response.StatusCode} ");
            sb.Append($"{request.Method} ");
            sb.Append($"{request.Path} ");
            if (!string.IsNullOrWhiteSpace(requestQueryString))
            {
                sb.Append($"RequestQuery: {requestQueryString} ");
            }
            if (!string.IsNullOrWhiteSpace(requestBodyString))
            {
                sb.Append($"Request: {requestBodyString} ");
            }
            sb.Append($"Response: {responseBodyString}");

            _logger.LogInformation(sb.ToString());
        }
    }
}