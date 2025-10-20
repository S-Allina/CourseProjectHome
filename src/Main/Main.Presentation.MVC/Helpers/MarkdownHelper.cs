
    using Markdig;
namespace Main.Presentation.MVC.Helpers
{
    public static class MarkdownHelper
    {
        private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        public static string ConvertToHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            return Markdown.ToHtml(markdown, _pipeline);
        }

        public static string TruncateWithMarkdown(string markdown, int maxLength = 150)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            // Удаляем Markdown разметку для превью
            var plainText = Markdown.ToPlainText(markdown, _pipeline);

            if (plainText.Length <= maxLength)
                return plainText;

            return plainText.Substring(0, maxLength) + "...";
        }
    }
}
