using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using App.Client.ApiServices;
using App.Shared;
using Components;
using MediatR;
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
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            ConfigureServices(builder.Services);
            await builder.Build().RunAsync();
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddApplicationComponents<ResourceManagerClientFactory>();
            services.AddAuthorizationCore();
            services.AddScoped<AuthServiceHttpClient>();
            services.AddScoped<IAuthService, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
            services.AddScoped<AuthenticationStateProvider, AuthServiceHttpClient>(provider => provider.GetRequiredService<AuthServiceHttpClient>());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IRequestHandler<,>), typeof(GenericRequestHandler<,>));
            services.AddTransient(typeof(INotificationHandler<>), typeof(GenericNotificationHandler<>));
        }
    }
}
