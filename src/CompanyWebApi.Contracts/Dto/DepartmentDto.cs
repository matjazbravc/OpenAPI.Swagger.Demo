using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Department Data Transfer Object
	/// </summary>
	[Serializable]
	public class DepartmentDto
	{
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
		public int DepartmentId { get; set; }

		public string Name { get; set; }

		public int CompanyId { get; set; }

        public string CompanyName { get; set; }

		public ICollection<string> Employees { get; set; } = new HashSet<string>();
	}
}
