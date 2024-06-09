using ChatApp.Application.Features.Accounts.Command.Register;
using FluentValidation;

namespace ChatApp.Application.Features.Accounts.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.UserName)
                .NotNull().WithMessage("User Name is required")
                .NotEmpty().WithMessage("User Name is required")
                .MinimumLength(3).WithMessage("User Name must be at least 3 characters long");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required")
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email address is not valid");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required")
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character");

            RuleFor(x => x.KnownAs)
                .NotNull().WithMessage("Known As is required")
                .NotEmpty().WithMessage("Known As is required");

            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Gender is required")
                .NotEmpty().WithMessage("Gender is required")
                .Must(gender => gender == "Male".ToLower() || gender == "Female".ToLower())
                .WithMessage("Gender must be either Male or Female");

            RuleFor(x => x.City)
                .NotNull().WithMessage("City is required")
                .NotEmpty().WithMessage("City is required");

            RuleFor(x => x.Country)
                .NotNull().WithMessage("Country is required")
                .NotEmpty().WithMessage("Country is required");

            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage("Date of Birth is required")
                .NotEmpty().WithMessage("Date of Birth is required")
                .Must(BeAtLeastValidAge).WithMessage("You must be at least 18 years old");
        }

        private bool BeAtLeastValidAge(DateTime dateOfBirth)
        {
            int currentYear = DateTime.Now.Year;
            int birthYear = dateOfBirth.Year;
            return currentYear - birthYear >= 18;
        }
    }
}
