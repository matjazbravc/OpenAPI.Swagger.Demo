using System;
using System.ComponentModel.DataAnnotations;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Company Creation Data Transfer Object
	/// </summary>
	[Serializable]
	public class CompanyUpdateDto
	{
		[Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
		public int CompanyId { get; set; }

		/// <summary>
		/// Company name
		/// </summary>
		/// <example>
		///     My Company
		/// </example>
        [Required]
        [StringLength(150, ErrorMessage = "Company name cannot be longer than 150 characters.")]
		public string Name { get; set; }
	}
}