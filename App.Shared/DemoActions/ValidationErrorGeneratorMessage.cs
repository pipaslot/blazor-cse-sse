using Pipaslot.Mediator.Abstractions;
using FluentValidation;

namespace App.Shared.DemoActions
{
    public static class ValidationErrorGeneratorMessage
    {
        public class Command : IMessage
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
