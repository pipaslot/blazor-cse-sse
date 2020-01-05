using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Shared.AuthModels;
using Core.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace App.Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> _authOptions;

        public AuthController(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions;
        }

        /// <summary>
        /// Authenticate user for JWT token
        /// </summary>
        [HttpPost("sign-in")]
        public SingInResult SignIn([FromBody] UserCredentials body)
        {
            var options = _authOptions.Value;
            var id = 5;
            var token = new JwtTokenBuilder()
                .AddSecurityKey(options.SecretKey)
                .AddSubject(body.Username)
                .AddIssuer(options.Issuer)
                .AddAudience(options.Audience)
                .AddClaim("MembershipId", id.ToString())
                //.AddClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role.ToString())
                .AddExpiry(options.TokenExpirationInMinutes)
                .Build();
            return new SingInResult
            {
                Success = true,
                AccessToken = token.Value,
                Username = body.Username
            };
        }

        /// <summary>
        /// Authenticate user with Cookie authentication
        /// </summary>
        [HttpGet("sign-in")]
        public async Task<ActionResult> SignInWithCookies(string username, string passwordHash, string returnUrl = "/")
        {
            //TODO unhash password
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                //new Claim("FullName", user.FullName),
                //new Claim(ClaimTypes.Role, "Administrator"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            return Redirect(returnUrl);
        }

        [HttpGet("sign-out")]
        public async Task<ActionResult> SignOut(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(returnUrl);
        }
    }
}