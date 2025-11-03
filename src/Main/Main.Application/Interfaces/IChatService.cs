using Main.Application.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatMessageDto> SaveMessageAsync(SendMessageDto messageDto);
        Task<List<ChatMessageDto>> GetMessageHistoryAsync(int inventoryId, int skip = 0, int take = 50);
        //Task<bool> EditMessageAsync(int messageId, string newMessage, string userId);
        //Task<bool> DeleteMessageAsync(int messageId, string userId);
    }
}
