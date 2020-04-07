using System;
using System.Diagnostics.CodeAnalysis;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Employee Data Transfer Object
	/// </summary>
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class EmployeeDto
	{
		public int EmployeeId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateTime BirthDate { get; set; }

		public int Age { get; set; }

		public string Company { get; set; }

		public string Department { get; set; }

		public string Address { get; set; }

		public string Username { get; set; }
	}
}