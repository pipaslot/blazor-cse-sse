using Pipaslot.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Mediator.Client
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register Mediator sending messages and requests over HTTPClient
        /// </summary>
        /// <param name="services">Service collection</param>
        public static IServiceCollection AddMediatorClient(this IServiceCollection services){
            services.AddSingleton<IMediator, ClientMediator>();
            return services;
        }
    }
}
