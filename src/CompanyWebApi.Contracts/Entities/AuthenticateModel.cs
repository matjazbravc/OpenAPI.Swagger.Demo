using System;
using System.ComponentModel.DataAnnotations;

namespace CompanyWebApi.Contracts.Entities
{
	/// <summary>
	/// Authenticate model
	/// </summary>
	[Serializable]
	public class AuthenticateModel
	{
        /// <summary>
        /// User name
        /// </summary>
        /// <example>alanf</example>
		[Required]
		public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        /// <example>test</example>
		[Required]
		public string Password { get; set; }
	}
}
