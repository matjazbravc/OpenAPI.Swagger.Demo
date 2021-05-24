using CompanyWebApi.Contracts.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Entities
{
	[Serializable]
	public class User : BaseAuditEntity
	{
		[Key, ForeignKey(nameof(Employee))]
		public int EmployeeId { get; set; }

		// Inverse navigation property
		public Employee Employee { get; set; }

		[Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User name cannot be longer than 150 characters.")]
		public string Username { get; set; }
	
		[Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User password cannot be longer than 50 characters.")]
		public string Password { get; set; }

		public string Token { get; set; }
	}
}
