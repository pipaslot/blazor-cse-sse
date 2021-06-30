using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class ValidationMiddleware : IRequestMiddleware, IEventMiddleware
    {
        private readonly ILogger<Program> _logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidationMiddleware(ILogger<Program> logger, IValidatorFactory validatorFactory)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
        }


        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, MiddlewareDelegate<TResponse> next) where TRequest : IRequest<TResponse>
        {
            await Validate(request, cancellationToken);

            return await next();
        }

        public async Task Handle<TEvent>(TEvent @event, CancellationToken cancellationToken, MiddlewareDelegate next) where TEvent : IEvent
        {
            await Validate(@event, cancellationToken);

            await next();
        }

        private async Task Validate<TTarget>(TTarget target, CancellationToken cancellationToken)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            var typeValidator = _validatorFactory.GetValidator(target.GetType());
            if (typeValidator != null)
            {
                var result = await typeValidator.ValidateAsync(new ValidationContext<object>(target), cancellationToken);

                if (result.Errors.Any())
                {
                    _logger.LogWarning("Model validation failed", result.Errors);
                    throw new ValidationException(result.Errors);
                }
            }
        }
    }
}