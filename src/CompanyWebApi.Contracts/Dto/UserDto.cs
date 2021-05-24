using System;

namespace CompanyWebApi.Contracts.Dto
{
    [Serializable]
    public class UserDto
	{
        public int EmployeeId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
