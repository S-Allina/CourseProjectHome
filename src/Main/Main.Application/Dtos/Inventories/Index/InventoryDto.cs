using Main.Application.Helpers;

namespace Main.Application.Dtos.Inventories.Index
{
    public record InventoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string DescriptionHtml => MarkdownHelper.ConvertToHtml(Description);
        public string DescriptionPreview => MarkdownHelper.TruncateWithMarkdown(Description);
        public int? CategoryId { get; init; }
        public string OwnerId { get; init; }
        public string Owner { get; init; }
        public string ImageUrl { get; init; }
        public bool IsPublic { get; init; }
        public string CustomIdFormat { get; init; }
        public byte[] Version { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Tags { get; init; } = new();
        public List<InventoryFieldDto> Fields { get; init; } = new();
        public List<InventoryAccessDto> AccessList { get; set; } = new();

    }
    // Statistics DTOs
    public class NumericFieldStats
    {
        public string FieldName { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public double Average { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Sum { get; set; }
        public int Count { get; set; }
        public double StandardDeviation { get; set; }
        public double Median { get; set; }

        // Для гистограммы/распределения
        public Dictionary<string, int> ValueDistribution { get; set; } = new();
        public List<double> Percentiles { get; set; } = new();
    }

    public class TextFieldStats
    {
        public string FieldName { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public int TotalValues { get; set; }
        public int UniqueValues { get; set; }
        public int EmptyValues { get; set; }

        // Самые частые значения
        public List<ValueFrequency> TopValues { get; set; } = new();
        public double AverageLength { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
    }

    public class BooleanFieldStats
    {
        public string FieldName { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public int TrueCount { get; set; }
        public int FalseCount { get; set; }
        public int TotalCount { get; set; }
        public double TruePercentage => TotalCount > 0 ? (double)TrueCount / TotalCount * 100 : 0;
        public double FalsePercentage => TotalCount > 0 ? (double)FalseCount / TotalCount * 100 : 0;
    }

    public class ValueFrequency
    {
        public string Value { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class DateFieldStats
    {
        public string FieldName { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public int TotalValues { get; set; }
        public int EmptyValues { get; set; }

        // Распределение по периодам
        public Dictionary<string, int> PeriodDistribution { get; set; } = new();
    }
    public class InventoryStatsDto
    {
        public int InventoryId { get; set; }
        public string InventoryName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Основная статистика
        public int TotalItems { get; set; }
        public int TotalFields { get; set; }
        public int ItemsWithImages { get; set; }
        public int ItemsWithComments { get; set; }
        public DateTime? OldestItemDate { get; set; }
        public DateTime? NewestItemDate { get; set; }

        // Распределение по типам полей
        public FieldTypeDistribution FieldTypeDistribution { get; set; } = new();

        // Детальная статистика по типам полей
        public List<NumericFieldStats> NumericFieldsStats { get; set; } = new();
        public List<TextFieldStats> TextFieldsStats { get; set; } = new();
        public List<BooleanFieldStats> BooleanFieldsStats { get; set; } = new();
        public List<DateFieldStats> DateFieldsStats { get; set; } = new();

        // Статистика активности
        public ActivityStats ActivityStats { get; set; } = new();

        // Самые популярные теги
        public List<ValueFrequency> TopTags { get; set; } = new();

        // Методы для вычисления процентов
        public double ItemsWithImagesPercentage => TotalItems > 0 ? (double)ItemsWithImages / TotalItems * 100 : 0;
        public double ItemsWithCommentsPercentage => TotalItems > 0 ? (double)ItemsWithComments / TotalItems * 100 : 0;
    }

    public class FieldTypeDistribution
    {
        public int NumericFieldsCount { get; set; }
        public int TextFieldsCount { get; set; }
        public int BooleanFieldsCount { get; set; }
        public int DateFieldsCount { get; set; }
        public int FileFieldsCount { get; set; }
        public int MultilineTextFieldsCount { get; set; }

        public int TotalFields => NumericFieldsCount + TextFieldsCount + BooleanFieldsCount +
                                 DateFieldsCount + FileFieldsCount + MultilineTextFieldsCount;
    }

    public class ActivityStats
    {
        public int ItemsCreatedLast7Days { get; set; }
        public int ItemsCreatedLast30Days { get; set; }
        public int ItemsUpdatedLast7Days { get; set; }
        public int CommentsAddedLast7Days { get; set; }
        public int LikesAddedLast7Days { get; set; }
    }
}
