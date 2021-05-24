using System.Collections.Generic;

namespace CompanyWebApi.Core.Errors
{
    public class ModelStateErrorResponse
    {
        public string Message { get; set; }

        public List<string> Errors { get; set; }
    }
}
