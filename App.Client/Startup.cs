using System;
using System.Linq;
using System.Net.Http;
using App.Client.Services;
using App.Shared;
using Blazor.Extensions.Logging;
using Components;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace App.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder
                .AddBrowserConsole());
            services.AddApplicationComponents();
            services.AddSingleton<IConfigProvider, ConfigProviderHttpClient>();
            services.AddSingleton<IAuthService, AuthServiceHttpClient>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Components.Application>("app");
        }
    }
}
