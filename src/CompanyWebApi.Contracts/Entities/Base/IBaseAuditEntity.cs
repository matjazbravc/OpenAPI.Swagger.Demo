using System;

namespace CompanyWebApi.Contracts.Entities.Base
{
	public interface IBaseAuditEntity
	{
		DateTime Created { get; set; }
		
		DateTime Modified { get; set; }
	}
}
