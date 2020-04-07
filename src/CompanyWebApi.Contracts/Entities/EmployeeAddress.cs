using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CompanyWebApi.Contracts.Entities
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	[JsonObject(IsReference = false)]
	public class EmployeeAddress
	{
		[Key, ForeignKey(nameof(Employee))]
		public int EmployeeId { get; set; }
		
        // Navigation property
		public Employee Employee { get; set; }

		public string Address { get; set; }
	}
}
