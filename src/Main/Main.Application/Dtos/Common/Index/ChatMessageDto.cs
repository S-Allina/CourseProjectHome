using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Dtos.Common.Index
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string? FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public string FormattedTime => CreatedAt.ToString("HH:mm");
        public string FormattedDate => CreatedAt.ToString("dd.MM.yyyy");
    }

    public class SendMessageDto
    {
        public int InventoryId { get; set; }
        public string Message { get; set; }
        public int? ParentMessageId { get; set; }
    }
}
