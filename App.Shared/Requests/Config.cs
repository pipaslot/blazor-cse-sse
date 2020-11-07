using MediatR;

namespace App.Shared.Requests
{
    
    public static class Config
    {
        public class Request : IRequest<Result>
        {
        
        }

        public class Result
        {
            public string Name { get; set; }
            public string[] Languages { get; set; }
        }
    }
}