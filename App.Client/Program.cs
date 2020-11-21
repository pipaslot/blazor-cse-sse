using System;
using System.Net.Http;
using System.Threading.Tasks;
using App.Client.ApiServices;
using App.Client.Resources;
using App.Shared;
using Blazored.LocalStorage;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Core.Localization;
using Core.Mediator;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace App.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Application>("app");
            builder.Services.AddTransient(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            ConfigureServices(builder.Services);
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureServerAndClientSharedServices<ResourceManagerClientFactory>(services);
            ConfigureOnlyClientSpecificServices(services);
        }
        
        public static void ConfigureServerAndClientSharedServices<TResourceManagerFactory>(IServiceCollection services)where TResourceManagerFactory : class, IResourceManagerFactory
        {
            services.AddOptions();

            //Register all resources
            services.AddCoreResources<TResourceManagerFactory>()
                .Register<LayoutResource>();

            services.AddStorage();
            services.AddFluxor(o =>
            {
                o.ScanAssemblies(typeof(Application).Assembly);
                o.UseReduxDevTools();
            });
            services.AddBlazoredLocalStorage();
        }

        private static void ConfigureOnlyClientSpecificServices(IServiceCollection services)
        {
            services.AddAuthorizationCore();
            
            services.AddScoped<AuthServiceHttpClient>();
            services.AddScoped<IAuthService, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
            services.AddScoped<AuthenticationStateProvider, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
            services.AddTransient<IMediator, SaveClientMediator>();
        }
    }
}
