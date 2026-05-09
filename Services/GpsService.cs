using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TransportApi.Data;
using TransportApi.Models;

namespace TransportApi.Services
{
    public class GpsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<DataHub> _hub;
        private readonly IHttpClientFactory _httpClientFactory;

        public GpsService(
            ApplicationDbContext context,
            IHubContext<DataHub> hub,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _hub = hub;
            _httpClientFactory = httpClientFactory;
        }

        // =========================
        // SEND SCHOOL LIVE DATA
        // =========================
        public async Task SendSchoolLiveData(int schoolId)
        {
            try
            {
                // =========================
                // GET SCHOOL TOKEN
                // =========================
                var setting = await _context.SchoolGpsSettings
                    .FirstOrDefaultAsync(x =>
                        x.SchoolId == schoolId &&
                        x.IsActive);

                if (setting == null)
                    return;

                // =========================
                // API URL
                // =========================
                var url =
                    $"{setting.ApiUrl}?accessToken={setting.AccessToken}";

                var client = _httpClientFactory.CreateClient();

                var response = await client.GetStringAsync(url);

                var json = JObject.Parse(response);

                var devices = json["object"] as JArray;

                if (devices == null || !devices.Any())
                    return;

                // =========================
                // LOOP DEVICES
                // =========================
                foreach (var device in devices)
                {
                    var deviceId = device["deviceUniqueId"]
                        ?.ToString()
                        ?.Trim()
                        ?.ToUpper();

                    if (string.IsNullOrWhiteSpace(deviceId))
                        continue;

                    var data = new
                    {
                        schoolId,
                        deviceId,

                        latitude = device["latitude"]
                            ?.ToObject<double>(),

                        longitude = device["longitude"]
                            ?.ToObject<double>(),

                        altitude = device["altitude"]
                            ?.ToObject<double>(),

                        speed = device["speed"]
                            ?.ToObject<double>(),

                        course = device["course"]
                            ?.ToObject<double>(),

                        ignition = device["attributes"]?["ignition"]
                            ?.ToObject<bool>(),

                        battery = device["attributes"]?["batteryLevel"]
                            ?.ToObject<double>(),

                        motion = device["attributes"]?["motion"]
                            ?.ToObject<bool>(),

                        name = device["name"]?.ToString(),

                        updatedAt = DateTime.UtcNow
                    };

                    // =========================
                    // GROUP NAME
                    // =========================
                    var groupName =
                        $"{schoolId}_{deviceId}";

                    // =========================
                    // SIGNALR SEND
                    // =========================
                    await _hub.Clients
                        .Group(groupName)
                        .SendAsync(
                            "ReceiveLocation",
                            data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}