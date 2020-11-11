using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Mediator;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class ValidationCommandPipeline<TRequest> : ICommandPipeline<TRequest>
    {
        private readonly ILogger<Program> _logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidationCommandPipeline(ILogger<Program> logger, IValidatorFactory validatorFactory)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
        }

        public async Task Handle(TRequest request, CancellationToken cancellationToken, QueryHandlerDelegate next)
        {
            var typeValidator = _validatorFactory.GetValidator(typeof(TRequest));
            if (typeValidator != null)
            {
                var result = await typeValidator.ValidateAsync(new ValidationContext<object>(request), cancellationToken);

                if (result.Errors.Any())
                {
                    _logger.LogWarning("Model validation failed", result.Errors);
                    throw new ValidationException(result.Errors);
                }
            }

            await next();
        }
    }

}