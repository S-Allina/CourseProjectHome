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

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
                .MaximumLength(20).WithMessage("Пароль не должен превышать 20 символов")
                .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру")
                .Matches("[^a-zA-Z0-9]").WithMessage("Пароль должен содержать хотя бы один спецсимвол");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Подтверждение пароля обязательно")
                .Equal(x => x.NewPassword).WithMessage("Пароли не совпадают");
        }
    }
}
