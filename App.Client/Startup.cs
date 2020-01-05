using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using App.Client.Services;
using App.Shared;
using Components;
using Components.Resources;
using Core.Localization;
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
            services.AddApplicationComponents<ResourceManagerClientFactory>();
            services.AddAuthorizationCore();
            services.AddScoped<IConfigProvider, ConfigProviderHttpClient>();
            services.AddScoped<AuthServiceHttpClient>();
            services.AddScoped<IAuthService, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
            services.AddScoped<AuthenticationStateProvider, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Application>("app");
        }
    }
}
