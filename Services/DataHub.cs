using Microsoft.AspNetCore.SignalR;

namespace TransportApi
{
    public class DataHub : Hub
    {
        public async Task JoinVehicleGroup(string vehicleNo)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, vehicleNo);
        }
    }
}