using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Company Data Transfer Object
	/// </summary>
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class CompanyDto
	{
		public int CompanyId { get; set; }

		public string Name { get; set; }

		public ICollection<string> Employees { get; set; } = new HashSet<string>();
	}
}