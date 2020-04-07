using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CompanyWebApi.Contracts.Entities
{
	/// <summary>
	/// Authenticate Entity
	/// </summary>
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class AuthenticateModel
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
