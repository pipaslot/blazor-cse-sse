using System.Threading;
using System.Threading.Tasks;
using App.Shared.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Server.RequestHandlers
{
    // ReSharper disable once UnusedType.Global
    public class ContactFormRequestHandler : IRequestHandler<ContactForm.Request, ContactForm.Result>
    {
        private readonly ILogger<ContactFormRequestHandler> _logger;

        public ContactFormRequestHandler(ILogger<ContactFormRequestHandler> logger)
        {
            _logger = logger;
        }

        public Task<ContactForm.Result> Handle(ContactForm.Request request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Contact form was processed");
            return Task.FromResult(new ContactForm.Result
            {
                Success = true
            });
        }
    }
}