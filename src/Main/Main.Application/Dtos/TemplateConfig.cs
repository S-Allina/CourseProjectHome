namespace Main.Application.Dtos
{
    public class TemplateConfig
    {
        public List<TemplateComponent> Components { get; set; } = new();
    }

    public class TemplateComponent
    {
        public string Type { get; set; } // "fixed", "random20", "sequence", etc.
        public string Value { get; set; } // Для fixed text, format strings
        public string Parameters { get; set; } // Дополнительные параметры
        public int Order { get; set; }
    }
}
