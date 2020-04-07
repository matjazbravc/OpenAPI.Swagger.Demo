using CompanyWebApi.Contracts.Entities.Base;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System;

namespace CompanyWebApi.Contracts.Entities
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	[JsonObject(IsReference = false)]
	public class User : BaseAuditEntity
	{
		[Key, ForeignKey(nameof(Employee))]
		public int EmployeeId { get; set; }

        // Navigation property
		public Employee Employee { get; set; }

		[Required]
		public string Username { get; set; }
	
		[Required]
		public string Password { get; set; }

		public string Token { get; set; }

        public override string ToString() => $"{EmployeeId}, {Username}";
	}
}
