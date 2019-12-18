using System;
using System.Linq;
using System.Net.Http;
using App.Client.Services;
using App.Shared;
using Blazor.Extensions.Logging;
using Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace App.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddLogging(builder => builder
            //    .AddBrowserConsole());
            services.AddApplicationComponents();
            services.AddAuthorizationCore();
            services.AddScoped<IConfigProvider, ConfigProviderHttpClient>();
            services.AddScoped<AuthServiceHttpClient>();
            services.AddScoped<IAuthService, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
            services.AddScoped<AuthenticationStateProvider, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Components.Application>("app");
        }
    }
}
