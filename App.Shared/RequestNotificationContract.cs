using System;
using System.Text.Json;

namespace App.Shared
{
    public class RequestNotificationContract
    {
        private RequestNotificationContract()
        {
        }

        public RequestNotificationContract(object query)
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