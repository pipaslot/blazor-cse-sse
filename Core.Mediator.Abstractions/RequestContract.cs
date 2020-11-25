using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Mediator.Abstractions
{
    public class RequestContract
    {
        private object _object;

        /// <summary>
        /// JSON deserialization constructor
        /// </summary>
        [JsonConstructor]
        public RequestContract()
        {
        }

        public RequestContract(object query)
        {
            Json = JsonSerializer.Serialize(query);
            ObjectName = query.GetType().AssemblyQualifiedName;
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Json { get; set; } = string.Empty;
        
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string ObjectName { get; set; } = string.Empty;

        public object GetObject() => _object ??= JsonSerializer.Deserialize(Json, Type.GetType(ObjectName));
    }
}