using AutoMapper;
using Main.Application.Dtos.Common;
using Main.Application.Interfaces;
using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
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
        private readonly IMapper _mapper;

        public ChatService(IChatRepository chatRepository, IUsersService usersService, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _usersService = usersService;
            _mapper = mapper;
        }
        
        public async Task<ChatMessageDto> SaveMessageAsync(SendMessageDto messageDto)
        {
            var user = await _usersService.GetCurrentUser();
            if (user == null) 
                throw new UnauthorizedAccessException("Чтобы отправить сообщение нужно авторизоваться.");

            var message = new ChatMessage
            {
                InventoryId = messageDto.InventoryId,
                UserId = user.Id,
                UserName=user.LastName + " " + user.FirstName,
                Message = messageDto.Message,
                CreatedAt = DateTime.UtcNow
            };

            await _chatRepository.CreateAsync(message);

            return _mapper.Map<ChatMessageDto>(message);
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

            return messages.Select(_mapper.Map<ChatMessageDto>).ToList();
        }
    }
}
