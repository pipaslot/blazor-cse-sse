using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.MediatorBehaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<Program> _logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidationBehavior(ILogger<Program> logger, IValidatorFactory validatorFactory)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
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

            return await next();
        }
    }

}