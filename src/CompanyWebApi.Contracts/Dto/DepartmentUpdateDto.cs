using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Department Data Transfer Object
	/// </summary>
	[Serializable]
	public class DepartmentUpdateDto
	{
		[Required]
        [Range(1, 9999, ErrorMessage = "Please enter a value bigger than {1}")]
		public int DepartmentId { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Department name cannot be longer than 150 characters.")]
		public string Name { get; set; }
	}
}
