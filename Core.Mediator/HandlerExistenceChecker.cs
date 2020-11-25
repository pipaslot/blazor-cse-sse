using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator
{
    public class HandlerExistenceChecker
    {
        private readonly List<Assembly> _subjectAssemblies = new List<Assembly>();
        private readonly IServiceProvider _services;

        public HandlerExistenceChecker(IServiceProvider services)
        {
            _services = services;
        }

        /// <summary>
        /// Verify that all commands and queries from the same assembly as specified type has registered handler.
        /// </summary>
        /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
        public HandlerExistenceChecker ScanFromAssemblyOf<T>()
        {
            _subjectAssemblies.Add(typeof(T).Assembly);
            return this;
        }

        /// <summary>
        /// Verify that all commands and queries from the same assembly as specified type has registered handler.
        /// </summary>
        public HandlerExistenceChecker ScanFromAssembly(params Assembly[] assemblies)
        {
            _subjectAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Scan registered assemblies for command and query types and try to resolve their handlers
        /// </summary>
        public void VerifyAll()
        {
            Verify<ICommand>("command");
            Verify<IQuery>("query");
            Verify<IRequest>("request");
        }

        private void Verify<T>(string subjectType)
        {
            var queryType = typeof(T);
            var queryTypes = _subjectAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsAbstract 
                            && p.IsClass 
                            && p.GetInterfaces().Any(i => i == queryType))
                .ToArray();
            using var scope = _services.CreateScope();
            var requestType = typeof(IRequest<>);
            foreach (var subject in queryTypes)
            {
                var resultType = subject
                    .GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestType)
                    .GetGenericArguments()
                    .First();
                var handlerType = typeof(IHandler<,>).MakeGenericType(subject, resultType);
                var handlers = scope.ServiceProvider.GetServices(handlerType).ToArray();
                if (handlers.Count() == 0)
                {
                    throw new Exception($"No handler was registered for {subjectType} type: {subject}");
                }
                if (handlers.Count() > 1)
                {
                    throw new Exception($"Multiple {subjectType} handlers were registered for one {subjectType} type: {subject} with classes {string.Join(" AND ", handlers)}");
                }
            }
        }
    }
}
