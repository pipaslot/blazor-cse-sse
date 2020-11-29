using Core.Mediator.Abstractions;
using FluentValidation;

namespace App.Shared.Commands
{
    public static class ValidationErrorGenerator
    {
        public class Command : ICommand
        {
            public string MandatoryParameter { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.MandatoryParameter).NotEmpty();
            }
        }
    }
}
