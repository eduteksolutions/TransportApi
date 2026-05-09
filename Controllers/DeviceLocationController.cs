using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Models;
using TransportApi.Models.TransportApi.Models;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<DataHub> _hub;

        public DeviceLocationController(
            ApplicationDbContext context,
            IHubContext<DataHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // =========================
        // 📡 UPDATE DEVICE LOCATION
        // =========================
        [HttpPost("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] DeviceLocationDto dto)
        {
            try
            {
                // ✅ VALIDATION
                if (string.IsNullOrWhiteSpace(dto.DeviceId))
                    return BadRequest("DeviceId is required");

                if (dto.Latitude < -90 || dto.Latitude > 90 ||
                    dto.Longitude < -180 || dto.Longitude > 180)
                {
                    return BadRequest("Invalid GPS coordinates");
                }

                var deviceId = dto.DeviceId.Trim().ToUpper();

                // =========================
                // 💾 SAVE TO DATABASE
                // =========================
                var entity = new VehicleDeviceLiveLocation
                {
                    SchoolId = dto.SchoolId,
                    DeviceId = deviceId,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Speed = dto.Speed,
                    Altitude = dto.Altitude,
                    Course = dto.Course,
                    BatteryLevel = dto.BatteryLevel,
                    Ignition = dto.Ignition,
                    Motion = dto.Motion,
                    UpdatedAt = DateTime.UtcNow,
                    RawData = dto.RawData
                };

                _context.VehicleDeviceLiveLocations.Add(entity);
                await _context.SaveChangesAsync();

                // =========================
                // 🚀 SIGNALR LIVE UPDATE
                // =========================
                await _hub.Clients
                    .Group(deviceId)
                    .SendAsync("ReceiveLocation", new
                    {
                        deviceId,
                        latitude = dto.Latitude,
                        longitude = dto.Longitude,
                        speed = dto.Speed,
                        altitude = dto.Altitude,
                        course = dto.Course,
                        batteryLevel = dto.BatteryLevel,
                        ignition = dto.Ignition,
                        motion = dto.Motion,
                        updatedAt = entity.UpdatedAt
                    });

                return Ok(new
                {
                    success = true,
                    message = "Device location updated successfully",
                    deviceId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // =========================
        // 📍 GET LATEST DEVICE LOCATION
        // =========================
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestLocation(int schoolId, string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                return BadRequest("DeviceId is required");

            var normalizedId = deviceId.Trim().ToUpper();

            var latest = await _context.VehicleDeviceLiveLocations
                .Where(x => x.SchoolId == schoolId && x.DeviceId == normalizedId)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No device location found"
                });
            }

            return Ok(new
            {
                success = true,
                deviceId = latest.DeviceId,
                latitude = latest.Latitude,
                longitude = latest.Longitude,
                speed = latest.Speed,
                altitude = latest.Altitude,
                course = latest.Course,
                batteryLevel = latest.BatteryLevel,
                ignition = latest.Ignition,
                motion = latest.Motion,
                updatedAt = latest.UpdatedAt
            });
        }
    }
}