using FluentValidation;
using Main.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Validators
{
    public class CreateInventoryFieldDtoValidator : AbstractValidator<CreateInventoryFieldDto>
    {
        public CreateInventoryFieldDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название поля обязательно")
                .MaximumLength(100).WithMessage("Название поля не может превышать 100 символов");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Описание поля не может превышать 500 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.FieldType)
                .IsInEnum().WithMessage("Некорректный тип поля");

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Порядковый индекс должен быть неотрицательным");
        }
    }
}
