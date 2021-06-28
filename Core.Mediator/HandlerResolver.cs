using Core.Mediator.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Core.Mediator
{
    public class HandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public HandlerResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        /// <summary>
        /// Get all registered handlers from service provider
        /// </summary>
        public object[] GetEventHandlers(Type eventType)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            return _serviceProvider.GetServices(handlerType)
                .Where(h => h != null)
                // ReSharper disable once RedundantEnumerableCastCall
                .Cast<object>()
                .ToArray();
        }

        /// <summary>
        /// Get all registered handlers from service provider
        /// </summary>
        public object[] GetRequestHandlers<TResponse>(Type requestType)
        {
            return GetRequestHandlers(requestType, typeof(TResponse));
        }


        /// <summary>
        /// Get all registered handlers from service provider
        /// </summary>
        public object[] GetRequestHandlers(Type requestType, Type responseType)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            return _serviceProvider.GetServices(handlerType)
                .Where(h => h != null)
                // ReSharper disable once RedundantEnumerableCastCall
                .Cast<object>()
                .ToArray();
        }
    }
}
