using CompanyWebApi.Services.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Middleware
{
    public class RequestResponseLoggingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Read and log request body data
            var requestBody = await ReadRequestBodyAsync(context.Request).ConfigureAwait(false);
            RequestLogHelper.RequestBody = requestBody;

            // Read and log response body data
            // Copy a pointer to the original response body stream
            var originalResponseBodyStream = context.Response.Body;

            // Create a new memory stream...
            await using var responseBody = new MemoryStream();
            // ...and use that for the temporary response body
            context.Response.Body = responseBody;

            // Continue down the Middleware pipeline, eventually returning to this class
            await next(context).ConfigureAwait(false);
            
            // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalResponseBodyStream).ConfigureAwait(false);
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            var body = request.Body;
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length)).ConfigureAwait(false);
            var requestBody = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            request.Body = body;
            return requestBody;
        }
    }
}
