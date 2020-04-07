using System;
using System.Dynamic;
using System.Net.Http;
using Xunit;

namespace CompanyWebApi.Tests.Services
{
    /// <summary>
    /// Base Controller tests IClassFixture
    /// </summary>
    public class ControllerTestsBase : IClassFixture<WebApiTestFactory>
    {
        protected HttpClient Client;
        protected dynamic Token;

        public ControllerTestsBase(WebApiTestFactory factory)
        {
            Client = factory.CreateClient();
            Token = new ExpandoObject();
            Token.sub = Guid.NewGuid();
            Token.role = new[] { "admin_role", "admin" };
        }
    }
}