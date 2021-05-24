using System;

namespace CompanyWebApi.Contracts.Dto
{
	/// <summary>
	/// Employee Search Data Transfer Object
	/// </summary>
    [Serializable]
	public class EmployeeSearchDto
	{
		/// <summary>
		/// Enter first name (or part of it)
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Enter last name (or part of it)
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Enter a birth date (yyyy-mm-dd)
		/// </summary>
		public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Enter department name (or part of it)
        /// </summary>
		public string Department { get; set; }

        /// <summary>
        /// Enter username (or part of it)
        /// </summary>
		public string Username { get; set; }
	}
}