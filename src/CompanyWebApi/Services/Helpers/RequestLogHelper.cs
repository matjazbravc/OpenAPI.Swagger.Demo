using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace CompanyWebApi.Services.Helpers
{
    public static class RequestLogHelper
    {
        public static string RequestBody { get; set; } = string.Empty;

        /// <summary>
        /// Enriches diagnostic context from request
        /// </summary>
        public static async void EnrichDiagnosticContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            diagnosticContext.Set("RequestBody", RequestBody);

            var responseBody = await ReadResponseBody(httpContext.Response).ConfigureAwait(false);
            diagnosticContext.Set("ResponseBody", responseBody);

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint != null) 
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"{responseBody}";
        }
    }
}
