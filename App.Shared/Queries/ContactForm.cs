using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.CQRSExtensions;
using FluentValidation;

namespace App.Shared.Queries
{
    public static class ContactForm
    {
        public class Query : IQuery<Result>
        {
            public string Firstname { get; set; } = "";
            public string Lastname { get; set; } = "";
            public string Email { get; set; } = "";
            public string Message { get; set; } = "";
        }

        public class Result
        {
            public bool Success { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(p => p.Firstname).NotEmpty().WithMessage("You must enter a name");
                RuleFor(p => p.Firstname).MaximumLength(50).WithMessage("Name cannot be longer than 50 characters");

                RuleFor(p => p.Lastname).NotEmpty().WithMessage("You must enter a name");
                RuleFor(p => p.Lastname).MaximumLength(50).WithMessage("Name cannot be longer than 50 characters");

                RuleFor(p => p.Email).NotEmpty().WithMessage("You must enter a email address");
                RuleFor(p => p.Email).EmailAddress().WithMessage("You must provide a valid email address").When(form => !string.IsNullOrEmpty(form.Email));

                RuleFor(p => p.Message).NotEmpty().WithMessage("Message is mandatory");
                
                RuleFor(x => x.Email)
                    .MustAsync(IsUniqueAsync)
                    .WithMessage("Name must be unique")
                    .When(form => !string.IsNullOrEmpty(form.Email));
            }

            private async Task<bool> IsUniqueAsync(string name, CancellationToken cancellationToken)
            {
                await Task.Delay(300);
                return name.ToLower() != "test@test.com";
            }
        }
    }
}