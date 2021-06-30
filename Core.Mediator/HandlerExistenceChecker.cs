using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Mediator.Abstractions;

namespace Core.Mediator
{
    public class HandlerExistenceChecker
    {
        private readonly List<Assembly> _subjectAssemblies = new List<Assembly>();
        /// <summary>
        /// We need to ignore handlers on less generic type. For example once command is catch, then we do not expect that generic IHandler will process that command as well.
        /// </summary>
        private readonly HashSet<Type> _alreadyVerified = new HashSet<Type>();
        private readonly ServiceResolver _handlerResolver;

        public HandlerExistenceChecker(ServiceResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
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

        public void Verify()
        {
            VerifyEvent();
            VerifyRequest();
        }

        private void VerifyEvent()
        {
            var eventInterface = typeof(IEvent);
            var subjectName = eventInterface.Name;
            var queryTypes = GetSubjects(eventInterface);
            foreach (var subject in queryTypes)
            {
                if (_alreadyVerified.Contains(subject))
                {
                    continue;
                }

                var handlers = _handlerResolver.GetEventHandlers(subject).ToArray();
                var middleware = _handlerResolver.GetExecutiveMiddleware(subject);
                VerifyHandlerCount(middleware, handlers, subject, subjectName);
                _alreadyVerified.Add(subject);
            }
        }

        private void VerifyRequest()
        {
            var requestInterface = typeof(IRequest);
            var subjectName = requestInterface.Name;
            var queryTypes = GetSubjects(requestInterface);
            foreach (var subject in queryTypes)
            {
                if (_alreadyVerified.Contains(subject))
                {
                    continue;
                }
                var resultType = Helpers.GetRequestResultType(subject);
                var handlers = _handlerResolver.GetRequestHandlers(subject, resultType);
                var middleware = _handlerResolver.GetExecutiveMiddleware(subject);
                VerifyHandlerCount(middleware, handlers, subject, subjectName);
                _alreadyVerified.Add(subject);
            }
        }
        private void VerifyHandlerCount(IExecutionMiddleware middleware, object[] handlers, Type subject, string subjectName)
        {
            if (handlers.Count() == 0)
            {
                throw new Exception($"No handler was registered for {subjectName} type: {subject}");
            }
            if (!middleware.ExecuteMultipleHandlers && handlers.Count() > 1)
            {
                throw new Exception($"Multiple {subjectName} handlers were registered for one {subjectName} type: {subject} with classes {string.Join(" AND ", handlers)}");
            }
        }

        private Type[] GetSubjects(Type type)
        {
            return _subjectAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass
                            && !p.IsAbstract
                            && !p.IsInterface
                            && p.GetInterfaces().Any(i => i == type))
                .ToArray();
        }
    }
}
