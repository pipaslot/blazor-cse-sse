using System.Text.Json;
using Core.Mediator.Abstractions;

namespace Core.Mediator
{
    public static class RequestContractFactory
    {

        public static RequestContract Create(object request)
        {
            return new RequestContract
            {
                Json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }),
                ObjectName = request.GetType().AssemblyQualifiedName
            };
        }
    }
}
