using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator
{
    public static class ServiceCollectionExtensions
    {
        public static MediatorConfigurator AddMediator(this IServiceCollection services)
        {
            services.AddTransient<HandlerResolver>();
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<HandlerExistenceChecker>();
            var configurator = new MediatorConfigurator(services);

            return configurator;
        }
    }
}
