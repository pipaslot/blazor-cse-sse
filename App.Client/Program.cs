using System;
using System.Net.Http;
using System.Threading.Tasks;
using App.Client.Resources;
using Blazored.LocalStorage;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Core.Localization;
using Core.Mediator.Abstractions;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Core.Mediator.Client;
using App.Client.Services;

namespace App.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Application>("#app");
            builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            ConfigureServices(builder.Services);
            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureServerAndClientSharedServices<ResourceManagerFactory>(services);
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
            
            services.AddScoped<AuthService>();
            services.AddScoped<AuthenticationStateProvider, AuthService>(provider => provider.GetRequiredService<AuthService>());
            services.AddSingleton<IMediator, ClientMediator>();
        }
    }
}
