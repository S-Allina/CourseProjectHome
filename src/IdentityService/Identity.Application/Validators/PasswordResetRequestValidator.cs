using FluentValidation;
using Identity.Application.Dto;

namespace Identity.Application.Validators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Токен обязателен");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email")
                .MaximumLength(100).WithMessage("Email не должен превышать 100 символов");
        }
    }
}
