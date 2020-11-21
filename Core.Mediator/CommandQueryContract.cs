using System;
using System.Text.Json;

namespace Core.Mediator
{
    public class CommandQueryContract
    {
        private CommandQueryContract()
        {
        }

        public CommandQueryContract(object query)
        {
            Json = JsonSerializer.Serialize(query);
            ObjectName = query.GetType().AssemblyQualifiedName;
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Json { get; set; } = string.Empty;
        
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string ObjectName { get; set; } = string.Empty;

        public object GetObject() => JsonSerializer.Deserialize(Json, Type.GetType(ObjectName));
    }
}