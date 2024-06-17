using ChatApp.Application.Features.Messages.Command.AddMessage;
using FluentValidation;

namespace ChatApp.Application.Features.Messages.Validator
{
    public class MessageValidator : AbstractValidator<AddMessageDto>
    {
        public MessageValidator()
        {
            RuleFor(x => x.RecipientUserName)
                            .NotNull().WithMessage("{PropertyName} must not be null.")
                            .NotEmpty().WithMessage("{PropertyName} must not be empty.");

            RuleFor(x => x.Content)
                .NotNull().WithMessage("{PropertyName} must not be null.")
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .MinimumLength(3).WithMessage("{PropertyName} must have at least {MinLength} characters.");
        }
    }
}
