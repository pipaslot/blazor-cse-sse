using Core.Mediator.Abstractions;

namespace App.Shared.Queries
{
    
    public static class Config
    {
        public class Query : IQuery<Result>
        {
        
        }

        public class Result
        {
            public string Name { get; set; }
            public string[] Languages { get; set; }
        }
    }
}