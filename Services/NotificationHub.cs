using Microsoft.AspNetCore.SignalR;

namespace TransportApi.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinStudentGroup(string admCd)
        {
            string groupName = $"Student_{admCd}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Connection {Context.ConnectionId} joined group: {groupName}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Connection {Context.ConnectionId} disconnected.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}