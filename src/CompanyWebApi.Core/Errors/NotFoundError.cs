using System.Diagnostics.CodeAnalysis;
using System.Net;
using CompanyWebApi.Core.Errors.Base;
using Microsoft.AspNetCore.Http;

namespace CompanyWebApi.Core.Errors
{
	[ExcludeFromCodeCoverage]
	public class NotFoundError : ApiError
	{
		public NotFoundError()
			: base(StatusCodes.Status404NotFound, HttpStatusCode.NotFound.ToString())
		{
		}
		
		public NotFoundError(string message)
			: base(StatusCodes.Status404NotFound, HttpStatusCode.NotFound.ToString(), message)
		{
		}
	}
}
