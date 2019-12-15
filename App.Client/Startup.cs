using System;
using System.Linq;
using System.Net.Http;
using Blazor.Extensions.Logging;
using Components;
using Core.Configuration;
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
            services.AddSingleton<IConfigProvider, HttpClientConfigProvider>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Components.App>("app");
        }
    }
}
