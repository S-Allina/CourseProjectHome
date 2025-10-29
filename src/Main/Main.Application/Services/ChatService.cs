using Main.Application.Dtos.Common.Index;
using Main.Application.Interfaces;
using Main.Domain.entities.common;
using Main.Domain.InterfacesRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUsersService _usersService;
        public ChatService(IChatRepository chatRepository, IUsersService usersService)
        {
            _chatRepository = chatRepository;
            _usersService = usersService;
        }

        public async Task<ChatMessageDto> SaveMessageAsync(SendMessageDto messageDto)
        {
            var user = await _usersService.GetCurrentUser();
            var message = new ChatMessage
            {
                InventoryId = messageDto.InventoryId,
                UserId = user.Id,
                UserName=user.LastName + " " + user.FirstName,
                Message = messageDto.Message,
                ParentMessageId = messageDto.ParentMessageId,
                CreatedAt = DateTime.UtcNow
            };

            await _chatRepository.CreateAsync(message);

            return MapToDto(message);
        }

        public async Task<List<ChatMessageDto>> GetMessageHistoryAsync(int inventoryId, int skip = 0, int take = 50)
        {
            var messages = await _chatRepository.GetAllAsync(m => m.InventoryId == inventoryId);
            messages.ToList();

              messages= messages.OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .OrderBy(m => m.CreatedAt)
                .ToList();

            return messages.Select(MapToDto).ToList();
        }

        private ChatMessageDto MapToDto(ChatMessage message)
        {
            return new ChatMessageDto
            {
                Id = message.Id,
                InventoryId = message.InventoryId,
                UserId = message.UserId,
                UserName = message.UserName,
                Message = message.Message,
                CreatedAt = message.CreatedAt,
                IsEdited = message.IsEdited
            };
        }
    }
}
