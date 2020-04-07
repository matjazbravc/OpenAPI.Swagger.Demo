using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;

namespace CompanyWebApi.Core.Auth
{
    [ExcludeFromCodeCoverage]
    public class JwtIssuerOptions
    {
        /// <summary>
        ///     Issuer - identifies the principal that issued the JWT
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        ///     Audience - identifies the recipients that the JWT is intended for
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        ///     Expiration - identifies the expiration time on or after which the JWT MUST NOT be accepted for processing
        /// </summary>
        public DateTime Expires => IssuedAt.Add(ValidFor);

        /// <summary>
        ///     Not Before - identifies the time before which the JWT MUST NOT be accepted for processing
        /// </summary>
        public DateTime NotBefore => DateTime.UtcNow;

        /// <summary>
        ///     Issued At - identifies the time at which the JWT was issued
        /// </summary>
        public DateTime IssuedAt => DateTime.UtcNow;

        /// <summary>
        ///     ValidFor - set the timespan the token will be valid for (default is 180 min)
        /// </summary>
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(180);

        /// <summary>
        ///     The signing key to use when generating tokens
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}