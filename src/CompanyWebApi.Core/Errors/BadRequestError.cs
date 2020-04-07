using System.Diagnostics.CodeAnalysis;
using System.Net;
using CompanyWebApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;

namespace CompanyWebApi.Core.Errors
{
	[ExcludeFromCodeCoverage]
	public class BadRequestError : ApiError
	{
		public BadRequestError()
			: base(StatusCodes.Status400BadRequest, HttpStatusCode.BadRequest.ToString())
		{
		}

		public BadRequestError(string message)
			: base(StatusCodes.Status400BadRequest, HttpStatusCode.BadRequest.ToString(), message)
		{
		}
	}
}
