using Main.Application.Dtos.Common;
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
            await Groups.AddToGroupAsync(Context.ConnectionId, $"inventory-{inventoryId}");

            var messageHistory = await _chatService.GetMessageHistoryAsync(inventoryId, 0, 50);

            await Clients.Caller.SendAsync("LoadMessageHistory", messageHistory);

            await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Successfully joined inventory group {inventoryId}");
        }

        public async Task SendMessage(int inventoryId, string message)
        {
            try
            {

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

                var savedMessage = await _chatService.SaveMessageAsync(mess);

                await Clients.Group($"inventory-{inventoryId}")
                    .SendAsync("ReceiveMessage", savedMessage);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex}");
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Error sending message: {ex.Message}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Connected with ID: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}