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
                    .FirstOrDefaultAsync(x => x.SchoolId == schoolId && x.IsActive);

                if (setting == null)
                    return;

                var url = $"{setting.ApiUrl}?accessToken={setting.AccessToken}";
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["object"] is not JArray devices || !devices.Any())
                    return;

                var logsToSave = new List<VehicleDeviceLiveLocation>();
                var signalRTasks = new List<Task>();
                var currentTime = DateTime.UtcNow;

                foreach (var device in devices)
                {
                    var deviceId = device["deviceUniqueId"]
                        ?.ToString()
                        ?.Trim()
                        ?.ToUpper();

                    if (string.IsNullOrWhiteSpace(deviceId))
                        continue;

                    // Extract status flags from your payload schema
                    bool isMoving = device["attributes"]?["motion"]?.Value<bool?>() ?? false;
                    bool hasIgnition = device["attributes"]?["ignition"]?.Value<bool?>() ?? false;
                    double speed = device["speed"]?.Value<double>() ?? 0.0;

                    var dto = new DeviceLocationDto
                    {
                        SchoolId = schoolId,
                        DeviceId = deviceId,
                        Latitude = device["latitude"]?.Value<double>() ?? 0,
                        Longitude = device["longitude"]?.Value<double>() ?? 0,
                        Speed = speed,
                        Altitude = device["altitude"]?.Value<double?>(),
                        Course = device["course"]?.Value<double?>(),
                        Ignition = hasIgnition,
                        BatteryLevel = device["attributes"]?["batteryLevel"]?.Value<double?>(),
                        Motion = isMoving,
                        RawData = device.ToString()
                    };

                    // ❌ skip invalid GPS
                    if (dto.Latitude == 0 || dto.Longitude == 0)
                        continue;

                    if (Math.Abs(dto.Latitude) > 90 || Math.Abs(dto.Longitude) > 180)
                        continue;

                    // ==========================================
                    // SIGNALR GROUP (Always broadcast to keep UI updated)
                    // ==========================================
                    var groupName = $"{schoolId}_{deviceId}";
                    signalRTasks.Add(_hub.Clients.Group(groupName).SendAsync("ReceiveLocation", dto));

                    // ==========================================
                    // SMART DATABASE FILTERS
                    // ==========================================

                    // 1. If payload explicitly reports stopped & ignition off, skip database logging
                    if (!isMoving && speed == 0 && !hasIgnition)
                        continue;

                    // 2. GPS Drift Fallback: Check the latest DB log to handle fractional static variations (e.g., 2.0 vs 2.00)
                    var lastLocation = await _context.VehicleDeviceLiveLocations
                        .Where(x => x.SchoolId == schoolId && x.DeviceId == deviceId)
                        .OrderByDescending(x => x.Timestamp)
                        .FirstOrDefaultAsync();

                    if (lastLocation != null)
                    {
                        const double epsilon = 0.00001; // Tiny threshold (~1 meter tolerance)
                        bool hasNotMoved = Math.Abs(lastLocation.Latitude - dto.Latitude) < epsilon &&
                                           Math.Abs(lastLocation.Longitude - dto.Longitude) < epsilon;

                        if (hasNotMoved)
                            continue; // Skip saving to database since position hasn't changed
                    }

                    // ==========================================
                    // SAVE TO DB (Only if actively driving/moved)
                    // ==========================================
                    logsToSave.Add(new VehicleDeviceLiveLocation
                    {
                        SchoolId = dto.SchoolId,
                        DeviceId = dto.DeviceId,
                        Latitude = dto.Latitude,
                        Longitude = dto.Longitude,
                        Speed = dto.Speed,
                        Course = dto.Course,
                        Altitude = dto.Altitude,
                        Timestamp = currentTime,
                        Date = currentTime.Date
                    });
                }

                // Run all UI broadcasts concurrently
                if (signalRTasks.Count > 0)
                {
                    await Task.WhenAll(signalRTasks);
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