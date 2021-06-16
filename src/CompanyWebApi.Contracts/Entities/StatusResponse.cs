using System;

namespace CompanyWebApi.Contracts.Entities
{
	public class StatusResponse
	{
		public string AssemblyName { get; set; }

		public string AssemblyVersion { get; set; }

		public string StartTime { get; set; }

		public string Host { get; set; }
	}
}
