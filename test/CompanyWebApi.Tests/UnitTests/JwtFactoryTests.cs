using CompanyWebApi.Core.Auth;
using CompanyWebApi.Tests.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using Xunit;

namespace CompanyWebApi.Tests.UnitTests
{
    public class JwtFactoryTests : IClassFixture<WebApiTestFactory>
    {
        [Fact]
        public void CanEncodeToken()
        {
            // Arrange
            var token = Guid.NewGuid().ToString();
            var jwtIssuerOptions = new JwtIssuerOptions
            {
                Issuer = "TEST ISSUER",
                Audience = "TEST AUDIENCE",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secretkey")), SecurityAlgorithms.HmacSha256)
            };

            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockJwtTokenHandler.Setup(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>())).Returns(token);
            var jwtFactory = new JwtFactory(mockJwtTokenHandler.Object, Options.Create(jwtIssuerOptions));

            // Act
            var result = jwtFactory.EncodeToken("userName");

            // Assert
            Assert.Equal(token, result);
        }
    }
}
