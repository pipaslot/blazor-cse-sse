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
            VerifyMessages();
            VerifyRequests();
        }

        private void VerifyMessages()
        {
            var subjectName = typeof(IMessage).Name;
            var queryTypes = GetMessageSubjects();
            foreach (var subject in queryTypes)
            {
                if (_alreadyVerified.Contains(subject))
                {
                    continue;
                }

                var handlers = _handlerResolver.GetMessageHandlers(subject).ToArray();
                var middleware = _handlerResolver.GetExecutiveMiddleware(subject);
                VerifyHandlerCount(middleware, handlers, subject, subjectName);
                _alreadyVerified.Add(subject);
            }
        }

        private void VerifyRequests()
        {
            var subjectName = typeof(IRequest<>).Name;
            var queryTypes = GetRequestSubjects();
            foreach (var subject in queryTypes)
            {
                if (_alreadyVerified.Contains(subject))
                {
                    continue;
                }
                var resultType = GenericHelpers.GetRequestResultType(subject);
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

        private Type[] GetMessageSubjects()
        {
            var types = _subjectAssemblies
                .SelectMany(s => s.GetTypes());
            return GenericHelpers.FilterAssignableToMessage(types);
        }

        private Type[] GetRequestSubjects()
        {
            var types = _subjectAssemblies
                .SelectMany(s => s.GetTypes());
            return GenericHelpers.FilterAssignableToRequest(types);
        }
    }
}
