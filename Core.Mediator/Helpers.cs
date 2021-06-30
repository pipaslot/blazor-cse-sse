using Core.Mediator.Abstractions;
using System;
using System.Linq;

namespace Core.Mediator
{
    internal static class Helpers
    {
        public static Type GetRequestResultType(Type? requestType)
        {
            if(requestType == null)
            {
                throw new ArgumentNullException(nameof(requestType));
            }
            var genericRequestType = typeof(IRequest<>);
            var genericInterface = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericRequestType);
            if(genericInterface == null)
            {
                throw new Exception($"Type {requestType} does not implements {genericRequestType}");
            }
            return genericInterface
                .GetGenericArguments()
                .First();
        }
    }
}
