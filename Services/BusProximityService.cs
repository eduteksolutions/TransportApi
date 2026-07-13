using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Hubs;
using TransportApi.Models;

namespace TransportApi.Services
{
    public class BusProximityService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _http;
        private readonly ILogger<BusProximityService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        private static readonly Dictionary<string, DateTime> _lastNotified = new();
        private static readonly TimeSpan CooldownPeriod = TimeSpan.FromMinutes(5);
        private const double ProximityMeters = 100.0;
        private const string NotificationBaseUrl = "https://eduteksolutions.in/info/";

        public BusProximityService(
            ApplicationDbContext context,
            HttpClient http,
            ILogger<BusProximityService> logger,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _http = http;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task CheckAndNotifyAsync(int schoolId, string deviceId, double busLat, double busLng)
        {
            try
            {
                var deviceIdClean = deviceId?.Trim().ToUpper() ?? "UNKNOWN";

                Console.WriteLine($"[TRACK] Starting proximity check for School: {schoolId}, Device: {deviceIdClean}");

                // 1. Fetch data by evaluating cross-table matching cleanly
                var studentStations = await (from student in _context.Admission
                                             from station in _context.TransportStationMaster
                                             where student.UserID == schoolId
                                                && student.PickupStationCd == station.StCode
                                                && student.UserID == station.UserID
                                             select new
                                             {
                                                 student.AdmCd,
                                                 student.UserID,
                                                 StudentName = student.FirstName,
                                                 StationName = station.StName,
                                                 Latitude = station.Latitude,
                                                 Longitude = station.Longitude
                                             })
                                             .ToListAsync();

                Console.WriteLine($"[TRACK POINT A] Query complete. Found {studentStations.Count} total records matching School ID {schoolId} in the database join.");

                if (!studentStations.Any())
                {
                    _logger.LogInformation("No student stations found matching School ID {SchoolId}", schoolId);
                    return;
                }

                bool standardLogsPending = false;

                foreach (var item in studentStations)
                {
                    Console.WriteLine($"[TRACK] Processing loop item for Student: {item.AdmCd}, Station: {item.StationName}");

                    // Null safety guard
                    if (item.Latitude == null || item.Longitude == null)
                    {
                        Console.WriteLine($"[TRACK WARNING] Skipped Student {item.AdmCd} because Latitude or Longitude is NULL in database.");
                        continue;
                    }

                    double stationLat = Convert.ToDouble(item.Latitude);
                    double stationLng = Convert.ToDouble(item.Longitude);

                    // 2. Calculate Distance
                    double distance = CalculateDistanceMeters(busLat, busLng, stationLat, stationLng);
                    _logger.LogInformation("Bus {Device} → Station {Station}: {Dist:F1}m", deviceIdClean, item.StationName, distance);

                    // 🔥 PRODUCTION GUARD: Skip tracking updates if the vehicle is outside the 100-meter boundary
                    if (distance > ProximityMeters)
                    {
                        Console.WriteLine($"[TRACK] Student {item.AdmCd} skipped. Bus is too far away ({distance:F0}m).");
                        continue;
                    }

                    // 4. Cooldown Check
                    var cooldownKey = $"{deviceIdClean}:{item.AdmCd}";
                    if (_lastNotified.TryGetValue(cooldownKey, out var lastSent) &&
                        DateTime.UtcNow - lastSent < CooldownPeriod)
                    {
                        Console.WriteLine($"[TRACK POINT B] Cooldown active for key {cooldownKey}. Skipping notification write.");
                        continue;
                    }

                    // 5. Channel A: Push SignalR real-time event
                    try
                    {
                        await _hubContext.Clients
                            .Group($"Student_{item.AdmCd}")
                            .SendAsync("StudentNotification", new
                            {
                                title = "Bus Arriving",
                                message = $"Bus is {Math.Round(distance)} meters from {item.StationName}",
                                admCd = item.AdmCd,
                                station = item.StationName,
                                deviceId = deviceIdClean
                            });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed sending SignalR message to Student_{Id}", item.AdmCd);
                    }

                    // Channel B: Push Rest API External Message
                    await SendExternalNotificationAsync(
                        admCd: item.AdmCd.ToString(),
                        userId: item.UserID?.ToString() ?? "0",
                        message: $"Your school bus is {distance:F0}m away from {item.StationName}! Please get ready.");

                    _lastNotified[cooldownKey] = DateTime.UtcNow;

                    // 7. Add Log entry to Context
                    _context.TransportBusProximityNotificationLogs.Add(new TransportBusProximityNotificationLogs
                    {
                        AdmCd = item.AdmCd.ToString(),
                        UserId = item.UserID?.ToString() ?? "0", // 🔥 FIX: Logs the correct School ID string cleanly now!
                        DeviceId = deviceIdClean,
                        BusLat = busLat,
                        BusLng = busLng,
                        StudentLat = stationLat,
                        StudentLng = stationLng,
                        DistanceMeters = distance,
                        SentAt = DateTime.UtcNow
                    });

                    Console.WriteLine($"[TRACK SUCCESS] Log row staged for Student {item.AdmCd}!");
                    standardLogsPending = true;
                }

                if (standardLogsPending)
                {
                    Console.WriteLine("[TRACK] Saving changes to SQL Server database...");
                    await _context.SaveChangesAsync();
                    Console.WriteLine("[TRACK COMPLETE] Database transaction committed successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pipeline context loops inside service execution");
            }
        }

        private async Task SendExternalNotificationAsync(string admCd, string userId, string message)
        {
            try
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                var url = $"{NotificationBaseUrl}api/SendPushNotificationAdmCdBy"
                        + $"?noticesubject=Notification"
                        + $"&noticetext={Uri.EscapeDataString(message)}"
                        + $"&sDate={today}"
                        + $"&userid={userId}"
                        + $"&admcd={admCd}";

                var response = await _http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("External push successfully dispatched to {Id}", admCd);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed forwarding Rest payload call to provider service framework");
            }
        }

        private double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                     * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}