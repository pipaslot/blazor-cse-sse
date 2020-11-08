using MediatR;

namespace App.Shared.Requests
{
    public static class ContactForm
    {
        public class Request : IRequest<Result>
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Email { get; set; }
            public string Message { get; set; }
        }

        public class Result
        {
            public bool Success { get; set; }
        }
    }
}