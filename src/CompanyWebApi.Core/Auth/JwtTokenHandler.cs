using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CompanyWebApi.Core.Auth
{
    [ExcludeFromCodeCoverage]
    public sealed class JwtTokenHandler : IJwtTokenHandler
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly ILogger<JwtTokenHandler> _logger;

        public JwtTokenHandler(ILogger<JwtTokenHandler> logger)
        {
            _logger = logger;
            if (_jwtSecurityTokenHandler == null)
            {
                _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            }
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                return null;
            }
        }

        public string WriteToken(JwtSecurityToken token)
        {
            return _jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}