using FluentValidation;
using Main.Application.Dtos.Inventories.Create;
using Main.Domain.enums.inventory;

namespace Main.Application.Validators
{
    public class CreateInventoryDtoValidator : AbstractValidator<CreateInventoryDto>
    {
        public CreateInventoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название инвентаря обязательно")
                .MaximumLength(200).WithMessage("Название не может превышать 200 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s\.\-_]+$").WithMessage("Название содержит запрещенные символы");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не может превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Категория обязательна");

            RuleFor(x => x.ImageUrl)
                .Must(BeAValidUrl).WithMessage("Некорректный URL изображения")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.Tags)
                .Must(tags => tags == null || tags.Count <= 10).WithMessage("Максимум 10 тегов")
                .ForEach(tagRule =>
                {
                    tagRule.MaximumLength(50).WithMessage("Тег не может превышать 50 символов");
                });

            //RuleFor(x => x.Fields)
            //.Must(ValidateFieldLimits).WithMessage("Превышены лимиты полей")
            //.When(x => x.Fields != null);

            RuleForEach(x => x.Fields)
                .SetValidator(new CreateInventoryFieldDtoValidator())
                .When(x => x.Fields != null);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        //private bool ValidateFieldLimits(List<CreateInventoryFieldDto> fields)
        //{
        //    if (fields == null || !fields.Any()) return true;

        //    var fieldCounts = fields.GroupBy(f => f.FieldType)
        //                           .ToDictionary(g => g.Key, g => g.Count());

        //    return fieldCounts.GetValueOrDefault(FieldType.Text) <= 3 &&
        //           fieldCounts.GetValueOrDefault(FieldType.MultilineText) <= 3 &&
        //           fieldCounts.GetValueOrDefault(FieldType.Number) <= 3 &&
        //           fieldCounts.GetValueOrDefault(FieldType.File) <= 3 &&
        //           fieldCounts.GetValueOrDefault(FieldType.Boolean) <= 3;
        //}
    }
}
