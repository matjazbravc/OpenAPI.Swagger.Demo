using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Department Data Transfer Object
	/// </summary>
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class DepartmentDto
	{
		public int DepartmentId { get; set; }

		public string Name { get; set; }
		
		public ICollection<string> Employees { get; set; } = new HashSet<string>();
	}
}
