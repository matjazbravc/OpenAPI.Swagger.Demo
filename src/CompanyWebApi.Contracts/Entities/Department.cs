using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using CompanyWebApi.Contracts.Entities.Base;
using Newtonsoft.Json;

namespace CompanyWebApi.Contracts.Entities
{
	// https://www.tektutorialshub.com/entity-framework-core-data-seeding/
	[Serializable]
	[ExcludeFromCodeCoverage]
	[JsonObject(IsReference = false)]
	public class Department : BaseAuditEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DepartmentId { get; set; }

		public string Name { get; set; }
		
        // Navigation property
		public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public override string ToString() => $"{DepartmentId}, {Name}";
	}
}
