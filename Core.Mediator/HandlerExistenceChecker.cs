using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Clear local state
        /// </summary>
        public void Clear()
        {
            _alreadyVerified.Clear();
        }

        /// <summary>
        /// Scan registered assemblies for command and query types and try to resolve their handlers.
        /// If subject was already checked, then is ignored in next rounds in case uf multiple invocations 
        /// </summary>
        /// <typeparam name="T">Subject</typeparam>
        /// <param name="subjectName">Subject name shown in exception in case of issue</param>
        /// <param name="checkOnlySingleHandlerIsRegistered">Check that at must one handler is registered</param>
        /// <returns></returns>
        public HandlerExistenceChecker VerifyEvent<T>(string subjectName, bool checkOnlySingleHandlerIsRegistered) where T : IEvent
        {
            var queryTypes = GetSubjects<T>();
            using var scope = _services.CreateScope();
            foreach (var subject in queryTypes)
            {
                if (_alreadyVerified.Contains(subject))
                {
                    continue;
                }

                var handlerType = GetEventHandlerType(subject);
                var handlers = scope.ServiceProvider.GetServices(handlerType).ToArray();
                if (handlers.Count() == 0)
                {
                    throw new Exception($"No handler was registered for {subjectName} type: {subject}");
                }
                if (checkOnlySingleHandlerIsRegistered && handlers.Count() > 1)
                {
                    throw new Exception($"Multiple {subjectName} handlers were registered for one {subjectName} type: {subject} with classes {string.Join(" AND ", handlers)}");
                }
                _alreadyVerified.Add(subject);
            }

            return this;
        }

        /// <summary>
        /// Scan registered assemblies for command and query types and try to resolve their handlers.
        /// If subject was already checked, then is ignored in next rounds in case uf multiple invocations 
        /// </summary>
        /// <typeparam name="T">Subject</typeparam>
        /// <param name="subjectName">Subject name shown in exception in case of issue</param>
        /// <param name="checkOnlySingleHandlerIsRegistered">Check that at must one handler is registered</param>
        /// <returns></returns>
        public HandlerExistenceChecker VerifyRequest<T>(string subjectName, bool checkOnlySingleHandlerIsRegistered) where T : IRequest
        {
            var queryTypes = GetSubjects<T>();
            using var scope = _services.CreateScope();
            foreach (var subject in queryTypes)
            {
                if (_alreadyVerified.Contains(subject))
                {
                    continue;
                }

                var handlerType = GetRequestHandlerType(subject);
                var handlers = scope.ServiceProvider.GetServices(handlerType).ToArray();
                if (handlers.Count() == 0)
                {
                    throw new Exception($"No handler was registered for {subjectName} type: {subject}");
                }
                if (checkOnlySingleHandlerIsRegistered && handlers.Count() > 1)
                {
                    throw new Exception($"Multiple {subjectName} handlers were registered for one {subjectName} type: {subject} with classes {string.Join(" AND ", handlers)}");
                }
                _alreadyVerified.Add(subject);
            }

            return this;
        }

        private Type[] GetSubjects<T>()
        {
            var type = typeof(T);
            return _subjectAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass
                            && !p.IsAbstract
                            && !p.IsInterface
                            && p.GetInterfaces().Any(i => i == type))
                .ToArray();
        }
        private Type GetEventHandlerType(Type subject)
        {
            return typeof(IEventHandler<>).MakeGenericType(subject);
        }

        private Type GetRequestHandlerType(Type subject)
        {
            var requestType = typeof(IRequest<>);
            var resultType = subject
                .GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestType)
                .GetGenericArguments()
                .First();
            return typeof(IRequestHandler<,>).MakeGenericType(subject, resultType);
        }
    }
}
