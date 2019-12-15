using System;
using System.Linq;
using System.Net.Http;
using Blazor.Extensions.Logging;
using Components;
using Components.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Optimiser.Blazor
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
