using CompanyWebApi.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace CompanyWebApi.Services.Helpers
{
    internal static class ConfigurationHelper
    {
        /// <summary>
        /// Gets Swagger options from appsettings
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static SwaggerOptions GetSwaggerOptions(IConfiguration config)
        {
            var swaggerSection = config.GetSection("Swagger");
            if (swaggerSection == null)
            {
                throw new FileLoadException("Swagger section not found in appsettings");
            }

            var result = config.GetSection("Swagger").GetSection("SwaggerOptions").Get<SwaggerOptions>();
            return result;
        }

        /// <summary>
        /// Gets list of Api versions from appsettings
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<SwaggerVersion> GetSwaggerVersions(IConfiguration config)
        {
            var swaggerSection = config.GetSection("Swagger");
            if (swaggerSection == null)
            {
                throw new FileLoadException("Swagger section not found in appsettings");
            }

            var result = config.GetSection("Swagger").GetSection("SwaggerVersions").Get<List<SwaggerVersion>>();
            return result;
        }

        /// <summary>
        /// Gets default api version from appsettings
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ApiVersion GetDefaultApiVersion(IConfiguration config)
        {
            var result = new ApiVersion(1, 0);
            var swaggerSection = config.GetSection("Swagger");
            if (swaggerSection == null)
            {
                throw new FileLoadException("Swagger section not found in appsettings");
            }

            var versions = config.GetSection("Swagger").GetSection("SwaggerVersions").Get<List<SwaggerVersion>>();
            var defVersion = versions.FirstOrDefault(v => v.Default);
            if (defVersion == null)
            {
                return result;
            }
            var version = ParseVersion(defVersion.Version.Replace("v", ""));
            if (version == null)
            {
                return result;
            }
            result = new ApiVersion(version.Major, version.Minor);

            return result;
        }

        private static Version ParseVersion(string input)
        {
            return Version.TryParse(input, out var version) ? version : null;
        }
    }
}