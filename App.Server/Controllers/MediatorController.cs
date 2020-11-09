using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using App.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IMediator = MediatR.IMediator;

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

        [HttpPost("request")]
        public async Task<ActionResult> MediatorRequest([FromBody]RequestNotificationContract commandQuery, CancellationToken cancellationToken)
        {
            var query = commandQuery.GetObject();
            var validationErrors = await Validate( query);
            if (validationErrors.Any())
            {
                return BadRequest(JsonSerializer.Serialize(validationErrors));
            }
            var result = await _mediator.Send(query, cancellationToken);
            return new JsonResult(result);
        }
        
        [HttpPost("notification")]
        public async Task<ActionResult> MediatorNotification([FromBody]RequestNotificationContract commandQuery, CancellationToken cancellationToken)
        {
            var query = commandQuery.GetObject();
            var validationErrors = await Validate( query);
            if (validationErrors.Any())
            {
                return BadRequest(JsonSerializer.Serialize(validationErrors));
            }
            await _mediator.Publish(query, cancellationToken);
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
