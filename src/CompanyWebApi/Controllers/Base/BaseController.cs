using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompanyWebApi.Controllers.Base
{
	// Inject common services in a BaseController
	public abstract class BaseController<T> : ControllerBase where T: BaseController<T>
	{
		private ILogger<T> _logger;

		protected ILogger<T> Logger => _logger ?? (_logger = HttpContext.RequestServices.GetService<ILogger<T>>());
	}
}
