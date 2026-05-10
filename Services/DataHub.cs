using Microsoft.AspNetCore.SignalR;

namespace TransportApi
{
    public class DataHub : Hub
    {
        // =========================
        // JOIN SCHOOL DEVICE GROUP
        // =========================
        public async Task JoinDeviceGroup(
            int schoolId,
            string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                return;

            var group =
                $"{schoolId}_{deviceId.Trim().ToUpper()}";

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                group);
        }

        // =========================
        // LEAVE GROUP
        // =========================
        public async Task LeaveDeviceGroup(
            int schoolId,


            string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                return;

            var group =
                $"{schoolId}_{deviceId.Trim().ToUpper()}";

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                group);
        }
    }
}