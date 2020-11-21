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
        private readonly CommandQueryContractExecutor _executor;
        private readonly IValidatorFactory _validatorFactory;

        public MediatorController(IMediator mediator, IValidatorFactory validatorFactory)
        {
            _executor = new CommandQueryContractExecutor(mediator);
            _validatorFactory = validatorFactory;
        }

        [HttpPost("query")]
        public async Task<ActionResult> MediatorQuery([FromBody]CommandQueryContract contract, CancellationToken cancellationToken)
        {
            var query = contract.GetObject();
            var validationErrors = await Validate( query);
            if (validationErrors.Any())
            {
                return BadRequest(JsonSerializer.Serialize(validationErrors));
            }

            var result = _executor.ExecuteQuery(contract, cancellationToken);
            return new JsonResult(result);
        }
        
        [HttpPost("command")]
        public async Task<ActionResult> MediatorCommand([FromBody]CommandQueryContract contract, CancellationToken cancellationToken)
        {
            var query = (ICommand)contract.GetObject();
            var validationErrors = await Validate( query);
            if (validationErrors.Any())
            {
                return BadRequest(JsonSerializer.Serialize(validationErrors));
            }

            await _executor.ExecuteCommand(contract, cancellationToken);
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
