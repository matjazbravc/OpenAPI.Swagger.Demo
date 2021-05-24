using System;

namespace CompanyWebApi.Contracts.Dto
{
    [Serializable]
    public class UserAuthenticateDto : UserDto
	{
        public string Token { get; set; }
	}
}
