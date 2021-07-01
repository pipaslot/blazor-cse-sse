using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Mediator.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class ValidationMiddleware : IMediatorMiddleware
    {
        private readonly ILogger<Program> _logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidationMiddleware(ILogger<Program> logger, IValidatorFactory validatorFactory)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
        }

        public async Task Invoke<TAction>(TAction action, MediatorResponse response, MiddlewareDelegate next, CancellationToken cancellationToken)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            var typeValidator = _validatorFactory.GetValidator(action.GetType());
            if (typeValidator != null)
            {
                var result = await typeValidator.ValidateAsync(new ValidationContext<object>(action), cancellationToken);

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