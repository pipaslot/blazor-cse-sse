using System.Threading;
using System.Threading.Tasks;
using App.Shared.Mediator;
using App.Shared.Queries;
using Microsoft.Extensions.Logging;

namespace App.Server.QueryHandlers
{
    // ReSharper disable once UnusedType.Global
    public class ContactFormQueryHandler : IQueryHandler<ContactForm.Query, ContactForm.Result>
    {
        private readonly ILogger<ContactFormQueryHandler> _logger;

        public ContactFormQueryHandler(ILogger<ContactFormQueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<ContactForm.Result> Handle(ContactForm.Query query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Contact form was processed");
            return Task.FromResult(new ContactForm.Result
            {
                Success = true
            });
        }
    }
}