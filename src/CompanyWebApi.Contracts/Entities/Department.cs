using CompanyWebApi.Contracts.Entities.Base;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Entities
{
	// https://www.tektutorialshub.com/entity-framework-core-data-seeding/
	[Serializable]
	[JsonObject(IsReference = false)]
	public class Department : BaseAuditEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required")]
		[StringLength(150, ErrorMessage = "Department name cannot be longer than 150 characters.")]
		public string Name { get; set; }

        [ForeignKey(nameof(Company))]
		public int CompanyId { get; set; }

		// Inverse navigation property
		public Company Company { get; set; }

        // Collection navigation property
		public IList<Employee> Employees { get; set; } = new List<Employee>();
	}
}
