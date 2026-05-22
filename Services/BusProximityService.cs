// Services/BusProximityService.cs
using TransportApi.Data;
using Microsoft.EntityFrameworkCore;
using TransportApi.Models;

namespace TransportApi.Services
{
    public class BusProximityService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _http;
        private readonly ILogger<BusProximityService> _logger;

        // Cooldown — don't re-notify same student+bus within 5 minutes
        private static readonly Dictionary<string, DateTime> _lastNotified = new();
        private static readonly TimeSpan CooldownPeriod = TimeSpan.FromMinutes(5);
        private const double ProximityMeters = 100.0;

        // Your existing notification API base URL
        private const string NotificationBaseUrl =
            "https://eduteksolutions.in/info/"; // 👈 replace with your real base URL

        public BusProximityService(
            ApplicationDbContext context,
            HttpClient http,
            ILogger<BusProximityService> logger)
        {
            _context = context;
            _http = http;
            _logger = logger;
        }

        public async Task CheckAndNotifyAsync(
            string deviceId,
            double busLat,
            double busLng)
        {
            try
            {
                // 1️⃣ Get latest location of all students assigned to this bus
                var students = await _context.StudentLocations
    .GroupBy(s => s.AdmCd)
    .Select(g => g.OrderByDescending(x => x.CreatedAt).First())
    .ToListAsync();

                if (!students.Any())
                {
                    _logger.LogInformation(
                        "No students found for device {DeviceId}", deviceId);
                    return;
                }

                foreach (var student in students)
                {
                    // 2️⃣ Calculate distance using Haversine formula
                    var distance = DistanceService.GetDistanceMeters(
                        busLat, busLng,
                        student.Latitude, student.Longitude);

                    _logger.LogInformation(
                        "Bus {DeviceId} → Student {AdmCd}: {Distance:F1}m",
                        deviceId, student.AdmCd, distance);

                    // 3️⃣ Skip if bus is not within 100 meters
                    if (distance > ProximityMeters) continue;

                    // 4️⃣ Cooldown check — prevent spam notifications
                    var cooldownKey = $"{deviceId}:{student.AdmCd}";
                    if (_lastNotified.TryGetValue(cooldownKey, out var lastSent) &&
                        DateTime.UtcNow - lastSent < CooldownPeriod)
                    {
                        _logger.LogInformation(
                            "Cooldown active for {Key}, skipping", cooldownKey);
                        continue;
                    }

                    // 5️⃣ Call your existing notification API
                    await SendExistingNotificationAsync(
                        admCd: student.AdmCd,
                        userId: student.UserId,
                        message: $"🚌 Your school bus is {distance:F0}m away! Please get ready.");

                    // 6️⃣ Update cooldown
                    _lastNotified[cooldownKey] = DateTime.UtcNow;

                    // 7️⃣ Log to DB
                    _context.TransportBusProximityNotificationLogs.Add(new TransportBusProximityNotificationLogs
                    {
                        AdmCd = student.AdmCd,
                        DeviceId = deviceId,
                        BusLat = busLat,
                        BusLng = busLng,
                        StudentLat = student.Latitude,
                        StudentLng = student.Longitude,
                        DistanceMeters = distance,
                        SentAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "✅ Notified student {AdmCd} — bus is {Distance:F0}m away",
                        student.AdmCd, distance);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ BusProximityService error");
            }
        }

        // ══════════════════════════════════════════════
        // Calls your existing SendPushNotificationAdmCdBy API
        // ══════════════════════════════════════════════
        private async Task SendExistingNotificationAsync(
            string admCd,
            string userId,
            string message)
        {
            try
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
              
                // Build exact same URL your Flutter app uses
                var url = $"{NotificationBaseUrl}api/SendPushNotificationAdmCdBy"
                        + $"?noticesubject=Notification"
                        + $"&noticetext={Uri.EscapeDataString(message)}"
                        + $"&sDate={today}"
                        + $"&userid={userId}"
                        + $"&admcd={admCd}";

                var response = await _http.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "✅ Notification sent to {AdmCd}: {Body}", admCd, body);
                }
                else
                {
                    _logger.LogWarning(
                        "⚠️ Notification API returned {Status} for {AdmCd}: {Body}",
                        response.StatusCode, admCd, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ SendExistingNotificationAsync failed for {AdmCd}", admCd);
            }
        }
    }
}