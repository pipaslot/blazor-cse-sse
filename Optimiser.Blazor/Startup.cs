//
//  Optimiser.Blazor.Startup
//
//  2019-04-18  Mark Stega
//              Created
//

using Blazor.Extensions.Logging;
using Components;
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
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Components.App>("app");
        }
    }
}
