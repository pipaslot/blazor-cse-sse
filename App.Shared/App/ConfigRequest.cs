using Pipaslot.Mediator.Abstractions;

namespace App.Shared.App
{
    
    public static class ConfigRequest
    {
        public class Query : IRequest<Result>
        {
        
        }

        public class Result
        {
            public string Name { get; set; }
            public string[] Languages { get; set; }
        }
    }
}