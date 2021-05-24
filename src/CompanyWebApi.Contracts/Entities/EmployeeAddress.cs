using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Entities
{
	[Serializable]
	public class EmployeeAddress
	{
		[Key, ForeignKey(nameof(Employee))]
		public int EmployeeId { get; set; }

		// Inverse navigation property
		public Employee Employee { get; set; }

		public string Address { get; set; }
	}
}
