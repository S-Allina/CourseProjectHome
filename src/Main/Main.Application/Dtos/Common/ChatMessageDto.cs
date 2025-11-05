using Main.Application.Helpers;

namespace Main.Application.Dtos.Common
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public required string UserId { get; set; }
        public string? UserName { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FormattedTime => CreatedAt.ToString("HH:mm");
        public string FormattedDate => CreatedAt.ToString("dd.MM.yyyy");
        public string MessageHtml => MarkdownHelper.ConvertToHtml(Message);

    }

    public class SendMessageDto
    {
        public int InventoryId { get; set; }
        public required string Message { get; set; }
    }
}
