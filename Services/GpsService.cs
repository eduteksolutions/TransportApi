using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TransportApi.Data;
using TransportApi.Models;
using TransportApi.Models.TransportApi.Models;

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
                var setting = await _context.SchoolGpsSettings
                    .FirstOrDefaultAsync(x =>
                        x.SchoolId == schoolId &&
                        x.IsActive);

                if (setting == null)
                    return;

                var url =
                    $"{setting.ApiUrl}?accessToken={setting.AccessToken}";

                var client = _httpClientFactory.CreateClient();

                var response = await client.GetStringAsync(url);

                var json = JObject.Parse(response);

                var devices = json["object"] as JArray;

                if (devices == null || !devices.Any())
                    return;

                var logsToSave = new List<VehicleDeviceLiveLocation>();

                foreach (var device in devices)
                {
                    var deviceId = device["deviceUniqueId"]
                        ?.ToString()
                        ?.Trim()
                        ?.ToUpper();

                    if (string.IsNullOrWhiteSpace(deviceId))
                        continue;

                    var dto = new DeviceLocationDto
                    {
                        SchoolId = schoolId,
                        DeviceId = deviceId,

                        Latitude = device["latitude"]?.Value<double>() ?? 0,
                        Longitude = device["longitude"]?.Value<double>() ?? 0,

                        Speed = device["speed"]?.Value<double?>(),
                        Altitude = device["altitude"]?.Value<double?>(),
                        Course = device["course"]?.Value<double?>(),

                        Ignition = device["attributes"]?["ignition"]?.Value<bool?>(),
                        BatteryLevel = device["attributes"]?["batteryLevel"]?.Value<double?>(),
                        Motion = device["attributes"]?["motion"]?.Value<bool?>(),

                        RawData = device.ToString()
                    };

                    // ❌ skip invalid GPS
                    if (dto.Latitude == 0 || dto.Longitude == 0)
                        continue;

                    if (Math.Abs(dto.Latitude) > 90 || Math.Abs(dto.Longitude) > 180)
                        continue;

                    // =========================
                    // SIGNALR GROUP
                    // =========================
                    var groupName = $"{schoolId}_{deviceId}";

                    await _hub.Clients
                        .Group(groupName)
                        .SendAsync("ReceiveLocation", dto);

                    // =========================
                    // SAVE TO DB (HISTORY)
                    // =========================
                    logsToSave.Add(new VehicleDeviceLiveLocation
                    {
                        SchoolId = dto.SchoolId,
                        DeviceId = dto.DeviceId,
                        Latitude = dto.Latitude,
                        Longitude = dto.Longitude,
                        Speed = dto.Speed,
                        Course = dto.Course,
                        Altitude = dto.Altitude,
                        Timestamp = DateTime.UtcNow,
                        Date = DateTime.UtcNow.Date
                    });
                }

                // =========================
                // BULK SAVE (IMPORTANT)
                // =========================
                if (logsToSave.Count > 0)
                {
                    await _context.VehicleDeviceLiveLocations.AddRangeAsync(logsToSave);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GPS SERVICE ERROR");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}