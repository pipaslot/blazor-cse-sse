using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Core.Jwt
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreAuthForClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCoreAuth(configuration, JwtBearerDefaults.AuthenticationScheme);
            return services;
        }

        public static IServiceCollection AddCoreAuthForServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCoreAuth(configuration, CookieAuthenticationDefaults.AuthenticationScheme);
            return services;
        }

        public static IServiceCollection AddCoreAuth(this IServiceCollection services, IConfiguration configuration, string authenticationScheme)
        {
            services.Configure<AuthOptions>(configuration);

            var authOptions = new AuthOptions();
            configuration.Bind(authOptions);
            services.AddAuthentication(authenticationScheme)
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
