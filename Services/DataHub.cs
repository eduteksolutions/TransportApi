using Microsoft.AspNetCore.SignalR;

namespace TransportApi
{
    public class DataHub : Hub
    {
        public async Task JoinVehicleGroup(string vehicleNo)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, vehicleNo);
        }
        public async Task JoinDeviceGroup(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId)) return;

            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId.Trim().ToUpper());
        }
    }
}