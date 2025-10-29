using Main.Application.Dtos.Common.Index;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Main.Application.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task JoinInventoryGroup(int inventoryId)
        {
            try
            {
                Console.WriteLine($"User {Context.ConnectionId} joining inventory group {inventoryId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, $"inventory-{inventoryId}");

                // Загружаем историю сообщений при присоединении к группе
                var messageHistory = await _chatService.GetMessageHistoryAsync(inventoryId, 0,50);
                await Clients.Caller.SendAsync("LoadMessageHistory", messageHistory);

                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Successfully joined inventory group {inventoryId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in JoinInventoryGroup: {ex}");
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Error joining group: {ex.Message}");
            }
        }

        public async Task SendMessage(int inventoryId, string message)
        {
            try
            {
                Console.WriteLine($"Received message from {Context.ConnectionId} for inventory {inventoryId}: {message}");

                // Проверяем валидность данных
                if (string.IsNullOrWhiteSpace(message))
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", "System", "Message cannot be empty");
                    return;
                }

                if (inventoryId <= 0)
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", "System", "Invalid inventory ID");
                    return;
                }

                var mess = new SendMessageDto
                {
                    Message = message,
                    InventoryId = inventoryId
                };
                // Сохраняем сообщение в БД
                var savedMessage = await _chatService.SaveMessageAsync(mess);

                // Отправляем сообщение всем в группе
                await Clients.Group($"inventory-{inventoryId}")
                    .SendAsync("ReceiveMessage", savedMessage);

                Console.WriteLine($"Message sent to group inventory-{inventoryId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex}");
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Error sending message: {ex.Message}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                Console.WriteLine($"Client connected: {Context.ConnectionId}");
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Connected with ID: {Context.ConnectionId}");
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnConnectedAsync: {ex}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                Console.WriteLine($"Client disconnected: {Context.ConnectionId}, Exception: {exception?.Message}");
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Disconnected");
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnDisconnectedAsync: {ex}");
            }
        }
    }
}