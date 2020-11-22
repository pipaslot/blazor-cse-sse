using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class ValidationCommandPipeline<TCommand> : ICommandPipeline<TCommand> where TCommand: notnull
    {
        private readonly ILogger<Program> _logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidationCommandPipeline(ILogger<Program> logger, IValidatorFactory validatorFactory)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
        }

        public async Task Handle(TCommand command, CancellationToken cancellationToken, CommandHandlerDelegate next)
        {
            var typeValidator = _validatorFactory.GetValidator(typeof(TCommand));
            if (typeValidator != null)
            {
                var result = await typeValidator.ValidateAsync(new ValidationContext<object>(command), cancellationToken);

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