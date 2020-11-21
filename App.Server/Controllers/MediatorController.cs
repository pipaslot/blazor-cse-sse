using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [Route("api/mediator")]
    [ApiController]
    public class MediatorController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidatorFactory _validatorFactory;

        public MediatorController(IMediator mediator, IValidatorFactory validatorFactory)
        {
            _mediator = mediator;
            _validatorFactory = validatorFactory;
        }

        [HttpPost("query")]
        public async Task<ActionResult> MediatorRequest([FromBody]RequestNotificationContract commandQuery, CancellationToken cancellationToken)
        {
            var query = commandQuery.GetObject();
            var validationErrors = await Validate( query);
            if (validationErrors.Any())
            {
                return BadRequest(JsonSerializer.Serialize(validationErrors));
            }

            var queryInterfaceType = typeof(IQuery<>);
            var resultType = query.GetType()
                .GetInterfaces()
                .FirstOrDefault(t=>t.GetGenericTypeDefinition() == queryInterfaceType)
                ?.GetGenericArguments()
                .FirstOrDefault();
            if (resultType == null)
            {
                throw new Exception($"Object {query.GetType()} is not assignable to type {queryInterfaceType }");
            }

            var method = _mediator.GetType()
                    .GetMethod(nameof(IMediator.Send))!
                .MakeGenericMethod(resultType);

            var task = (Task)method.Invoke(_mediator, new[] {query, cancellationToken})!;
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            var result= resultProperty?.GetValue(task);
            return new JsonResult(result);
        }
        
        [HttpPost("command")]
        public async Task<ActionResult> MediatorNotification([FromBody]RequestNotificationContract commandQuery, CancellationToken cancellationToken)
        {
            var query = (ICommand)commandQuery.GetObject();
            var validationErrors = await Validate( query);
            if (validationErrors.Any())
            {
                return BadRequest(JsonSerializer.Serialize(validationErrors));
            }
            await _mediator.Dispatch(query, cancellationToken);
            return Ok();
        }

        private async Task<IReadOnlyList<ValidationErrorDto>> Validate(object command)
        {
            var errors = new List<ValidationErrorDto>();
            var typeValidator = _validatorFactory.GetValidator(command.GetType());
            if (typeValidator != null)
            {
                var result = await typeValidator.ValidateAsync(new ValidationContext<object>(command));
                errors.AddRange(result.Errors.Select(o => new ValidationErrorDto
                {
                    ErrorMessage = o.ErrorMessage,
                    PropertyName = o.PropertyName
                }));
            }

            return errors;
        }


        public class ValidationErrorDto
        {
            public string PropertyName { get; set; } = string.Empty;
            public string ErrorMessage { get; set; } = string.Empty;
        }

    }
}
