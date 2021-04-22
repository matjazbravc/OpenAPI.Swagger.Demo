using System.Diagnostics.CodeAnalysis;

namespace CompanyWebApi.Core.Errors.Base
{
    [ExcludeFromCodeCoverage]
    public class ApiError
    {
        public ApiError(int statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        public ApiError(int statusCode, string statusDescription, string message)
            : this(statusCode, statusDescription)
        {
            Message = message;
        }

        public ApiError(int statusCode, string statusDescription, string message, string requestPath)
            : this(statusCode, statusDescription, message)
        {
            RequestPath = requestPath;
        }

        public string Message { get; private set; }

        public int StatusCode { get; }

        public string StatusDescription { get; }

        public string RequestPath { get; }
    }
}