using CompanyWebApi.Contracts.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Entities
{
	[Serializable]
	public class Company : BaseAuditEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CompanyId { get; set; }

		[Required(ErrorMessage = "Company name is required")]
        [StringLength(150, ErrorMessage = "Company name cannot be longer than 150 characters.")]
		public string Name { get; set; }

		// Collection navigation property
		public IList<Department> Departments { get; set; } = new List<Department>();
	}
}