using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace CompanyWebApi.Services.Controllers
{
    /// <summary>
    /// Adds a convention to let Swagger understand the different API versions
    /// </summary>
    public class GroupingByNamespaceConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace;
            var apiVersion = controllerNamespace?.Split(".").Last().ToLower();
            if (apiVersion == null || !apiVersion.StartsWith("v"))
            {
                apiVersion = "v1";
            }
            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}
