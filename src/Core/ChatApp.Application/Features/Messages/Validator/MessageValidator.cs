using ChatApp.Application.Features.Messages.Command.AddMessage;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Messages.Validator
{
    public class MessageValidator : AbstractValidator<AddMessageDto>
    {
        public MessageValidator()
        {
            RuleFor(x => x.Content).NotNull().WithMessage("{PropertyName} is not null")
                .MinimumLength(3).WithMessage("{PropertyName} Min Length {PropertyValue}");
        }
    }
}
