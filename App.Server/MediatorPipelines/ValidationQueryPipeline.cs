using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class ValidationQueryPipeline<TRequest, TResponse> : IQueryPipeline<TRequest, TResponse>
    {
        private readonly ILogger<Program> _logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidationQueryPipeline(ILogger<Program> logger, IValidatorFactory validatorFactory)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, QueryHandlerDelegate<TResponse> next)
        {
            var typeValidator = _validatorFactory.GetValidator(request.GetType());
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