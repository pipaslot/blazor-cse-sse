using System.Threading.Tasks;
using App.Client.Services;
using App.Shared;
using Components;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace App.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Application>("app");
            ConfigureServices(builder.Services);
            await builder.Build().RunAsync();
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            //services.AddLogging(builder => builder
            //    .AddBrowserConsole());
            services.AddOptions();
            services.AddApplicationComponents<ResourceManagerClientFactory>();
            services.AddAuthorizationCore();
            services.AddScoped<IConfigProvider, ConfigProviderHttpClient>();
            services.AddScoped<AuthServiceHttpClient>();
            services.AddScoped<IAuthService, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
            services.AddScoped<AuthenticationStateProvider, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
        }
    }
}
