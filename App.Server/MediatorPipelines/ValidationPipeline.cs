using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class ValidationPipeline: IRequestPipeline
    {
        private readonly ILogger<Program> _logger;
        private readonly IValidatorFactory _validatorFactory;

        public ValidationPipeline(ILogger<Program> logger, IValidatorFactory validatorFactory)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
        }

        
        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) where TRequest : IRequest<TResponse>
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