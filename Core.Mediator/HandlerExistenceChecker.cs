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
            VerifyCommands();
            VerifyQueries();
        }

        private void VerifyCommands()
        {
            var commandType = typeof(ICommand);
            var commandTypes = _subjectAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsAbstract && p.IsClass && commandType.IsAssignableFrom(p))
                .ToArray();
            using var scope = _services.CreateScope();
            foreach (var subject in commandTypes)
            {
                var handlerType = typeof(ICommandHandler<>).MakeGenericType(subject);
                var handlers = scope.ServiceProvider.GetServices(handlerType);
                CheckCount(handlers, subject, "command");
            }
        }

        private void VerifyQueries()
        {
            var queryType = typeof(IQuery<>);
            var queryTypes = _subjectAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsAbstract && p.IsClass && p.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == queryType))
                .ToArray();
            using var scope = _services.CreateScope();
            foreach (var subject in queryTypes)
            {
                var resultType = subject
                    .GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == queryType)
                    .GetGenericArguments()
                    .First();
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(subject, resultType);
                var handlers = scope.ServiceProvider.GetServices(handlerType);
                CheckCount(handlers, subject, "query");
            }
        }

        private void CheckCount(IEnumerable<object?> handlers, Type subject, string sujectType)
        {
            var count = handlers.Count();
            if (count == 0)
            {
                throw new Exception($"No handler was registered for {sujectType} type: {subject}");
            }
            if (count > 1)
            {
                throw new Exception($"Multiple {sujectType} handlers were registered for one {sujectType} type: {subject} with classes {string.Join(" AND ", handlers)}");
            }
        }
    }
}
