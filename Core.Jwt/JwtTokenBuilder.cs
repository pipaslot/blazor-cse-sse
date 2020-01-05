using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core.Jwt
{
    public sealed class JwtTokenBuilder
    {
        private SecurityKey _securityKey;
        private string _subject = "";
        private string _issuer = "";
        private string _audience = "";
        private readonly Dictionary<string, string> _claims = new Dictionary<string, string>();
        private int _expiryInMinutes = 120;


        public JwtTokenBuilder AddSecurityKey(SecurityKey securityKey)
        {
            _securityKey = securityKey;
            return this;
        }
        public JwtTokenBuilder AddSecurityKey(string securityKey)
        {
            _securityKey = CreateSymetricKey(securityKey);
            return this;
        }

        public JwtTokenBuilder AddSubject(string subject)
        {
            _subject = subject;
            return this;
        }

        public JwtTokenBuilder AddIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public JwtTokenBuilder AddAudience(string audience)
        {
            _audience = audience;
            return this;
        }

        public JwtTokenBuilder AddClaim(string type, string value)
        {
            _claims.Add(type, value);
            return this;
        }

        public JwtTokenBuilder AddClaims(Dictionary<string, string> claims)
        {
            _claims.Union(claims);
            return this;
        }

        public JwtTokenBuilder AddExpiry(int expiryInMinutes)
        {
            _expiryInMinutes = expiryInMinutes;
            return this;
        }

        public JwtToken Build()
        {
            EnsureArguments();

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }
                .Union(_claims.Select(item => new Claim(item.Key, item.Value)));

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryInMinutes),
                signingCredentials: new SigningCredentials(
                    _securityKey,
                    SecurityAlgorithms.HmacSha256));

            return new JwtToken(token.ValidTo, new JwtSecurityTokenHandler().WriteToken(token));
        }

        public static SymmetricSecurityKey CreateSymetricKey(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }

        #region Private

        private void EnsureArguments()
        {
            if (this._securityKey == null)
            {
                throw new ArgumentNullException("Security Key");
            }

            if (string.IsNullOrWhiteSpace(this._subject))
            {
                throw new ArgumentNullException("Subject");
            }

            if (string.IsNullOrWhiteSpace(this._issuer))
            {
                throw new ArgumentNullException("Issuer");
            }

            if (string.IsNullOrWhiteSpace(this._audience))
            {
                throw new ArgumentNullException("Audience");
            }
        }

        #endregion

    }
}