using System;
using Newtonsoft.Json;

namespace CompanyWebApi.Contracts.Entities.Base
{
	public abstract class BaseAuditEntity : IBaseAuditEntity    
	{
		public DateTime Created { get; set; } = DateTime.UtcNow;
		
		public DateTime Modified { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
	}
}
