using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Dto
{
    [Serializable]
	public class UserCreateDto
	{
        /// <summary>
        /// Employee Id
        /// </summary>
        /// <example>6</example>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int EmployeeId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        /// <example>alanf</example>
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User name cannot be longer than 50 characters.")]
        public string Username { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        /// <example>test</example>
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User password cannot be longer than 50 characters.")]
        public string Password { get; set; }
    }
}
