using CompanyWebApi.Core.Errors;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CompanyWebApi.Services.Filters
{
    /// <summary>
    /// Action filter which checks if ModelState is valid
    /// </summary>
    public class ValidModelStateAsyncActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<ValidModelStateAsyncActionFilter> _logger;
        private static readonly Action<ILogger, IList<(string Key, string ErrorMessage, string ExceptionMessage)>, Exception> ModelStateLoggerAction;
        
        static ValidModelStateAsyncActionFilter()
        {
            ModelStateLoggerAction = LoggerMessage.Define<IList<(string Key, string ErrorMessage, string ExceptionMessage)>>(LogLevel.Warning, new EventId(1, nameof(ValidModelStateAsyncActionFilter)), "{ModelState}");
        }

        public ValidModelStateAsyncActionFilter(ILogger<ValidModelStateAsyncActionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                await next();
            }
            LogModelState(context);
            context.Result = new BadRequestObjectResult(GetErrorResponse(context));
        }

        private static ModelStateErrorResponse GetErrorResponse(ActionContext context)
        {
            return new()
            {
                Message = "Validation Error. The input paramaters are invalid.",
                Errors = context.ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList()
            };
        }

        private void LogModelState(ActionContext context)
        {
            var items = from ms in context.ModelState
                        where ms.Value.Errors.Any()
                        let fieldKey = ms.Key
                        let errors = ms.Value.Errors
                        from error in errors
                        select (Key: fieldKey, error.ErrorMessage, ExceptionMessage: error.Exception?.Message);
            var result = items.ToList();
            if (result.Any())
            {
                ModelStateLoggerAction(_logger, result, null);
            }
        }
    }
}