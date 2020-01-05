using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Core.Jwt
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreAuth(this IServiceCollection services, IConfiguration configuration, bool isClientSide)
        {
            services.Configure<AuthOptions>(configuration);

            var authOptions = new AuthOptions();
            configuration.Bind(authOptions);
            services.AddAuthentication(isClientSide ? JwtBearerDefaults.AuthenticationScheme : CookieAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = authOptions.Issuer,
                            ValidAudience = authOptions.Audience,
                            IssuerSigningKey = JwtTokenBuilder.CreateSymetricKey(authOptions.SecretKey),
                        };
                })
                .AddCookie( CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(5.0);
                });
            return services;
        }
    }
}
