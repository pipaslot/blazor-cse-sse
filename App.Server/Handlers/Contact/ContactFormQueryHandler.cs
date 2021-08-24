using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using App.Shared.Contact;

namespace App.Server.Handlers.Contact
{
    // ReSharper disable once UnusedType.Global
    public class ContactFormQueryHandler : IRequestHandler<ContactFormRequest.Query, ContactFormRequest.Result>
    {
        private readonly ILogger<ContactFormQueryHandler> _logger;

        public ContactFormQueryHandler(ILogger<ContactFormQueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<ContactFormRequest.Result> Handle(ContactFormRequest.Query query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Contact form was processed");
            return Task.FromResult(new ContactFormRequest.Result
            {
                Success = true
            });
        }
    }
}