namespace Core.Mediator.Abstractions
{
    public class RequestContract
    {
        public const string Endpoint = "/api/mediator/request";
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Json { get; set; } = string.Empty;
        
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string ObjectName { get; set; } = string.Empty;
    }
}