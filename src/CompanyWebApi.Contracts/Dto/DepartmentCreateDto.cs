using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Department Data Transfer Object
	/// </summary>
	[Serializable]
	public class DepartmentCreateDto
	{
		/// <summary>
		/// Company id
		/// </summary>
        [Required]
        public int CompanyId { get; set; }

		/// <summary>
		/// Department name
		/// </summary>
        [Required(ErrorMessage = "Department name is required")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Department name cannot be longer than 150 characters.")]
		public string Name { get; set; }
	}
}
