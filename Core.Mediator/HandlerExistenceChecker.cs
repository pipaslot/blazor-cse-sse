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
        /// <summary>
        /// We need to ignore handlers on less generic type. For example once command is catch, then we do not expect that generic IHandler will process that command as well.
        /// </summary>
        private readonly HashSet<Type> _alreadyVerified = new HashSet<Type>();

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
        public virtual void VerifyAll()
        {
            _alreadyVerified.Clear();
            Verify<ICommand>("command", true);
            Verify<IQuery>("query", true);
            Verify<IRequest>("request", true);
        }

        protected void Verify<T>(string subjectType, bool checkOnlySingleHandlerIsRegistered)
        {
            var queryTypes = GetSubjects<T>();
            using var scope = _services.CreateScope();
            foreach (var subject in queryTypes)
            {
                if (_alreadyVerified.Contains(subject))
                {
                    continue;
                }

                var handlerType = GetHandlerType(subject);
                var handlers = scope.ServiceProvider.GetServices(handlerType).ToArray();
                if (handlers.Count() == 0)
                {
                    throw new Exception($"No handler was registered for {subjectType} type: {subject}");
                }
                if (checkOnlySingleHandlerIsRegistered && handlers.Count() > 1)
                {
                    throw new Exception($"Multiple {subjectType} handlers were registered for one {subjectType} type: {subject} with classes {string.Join(" AND ", handlers)}");
                }
                _alreadyVerified.Add(subject);
            }
        }

        private Type[] GetSubjects<T>()
        {
            var type = typeof(T);
            return _subjectAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsAbstract 
                            && p.IsClass 
                            && p.GetInterfaces().Any(i => i == type))
                .ToArray();
        }

        private Type GetHandlerType(Type subject)
        {
            var requestType = typeof(IRequest<>);
            var resultType = subject
                .GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestType)
                .GetGenericArguments()
                .First();
            return typeof(IHandler<,>).MakeGenericType(subject, resultType);
        }
    }
}
