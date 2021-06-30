namespace Core.Mediator.Abstractions
{
    public class MediatorRequest
    {
        public const string Endpoint = "/_mediator/request";
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Json { get; set; } = string.Empty;
        
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string ObjectName { get; set; } = string.Empty;
    }
}