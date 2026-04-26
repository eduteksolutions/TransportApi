using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<DataHub> _hub;

        public LocationController(ApplicationDbContext context, IHubContext<DataHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // 🚀 DRIVER: Save live location + push realtime
        [HttpPost("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] LiveLocationDto dto)
        {
            if (string.IsNullOrEmpty(dto.VehicleNo))
                return BadRequest("VehicleNo is required");

            if (dto.Latitude < -90 || dto.Latitude > 90 ||
                dto.Longitude < -180 || dto.Longitude > 180)
            {
                return BadRequest("Invalid coordinates");
            }

            var entity = new VehicleLiveLocation
            {
                SchoolId = dto.SchoolId,
                VehicleNo = dto.VehicleNo,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Speed = dto.Speed,
                UpdatedAt = DateTime.Now
            };

            _context.VehicleLiveLocations.Add(entity);
            await _context.SaveChangesAsync();

            // ✅ FIX: Send CLEAN OBJECT (NOT DTO)
            await _hub.Clients.All.SendAsync("ReceiveLocation", new
            {
                schoolId = dto.SchoolId,
                vehicleNo = dto.VehicleNo,
                latitude = dto.Latitude,
                longitude = dto.Longitude,
                speed = dto.Speed,
                updatedAt = entity.UpdatedAt
            });

            return Ok(new
            {
                message = "Live location updated",
                data = dto
            });
        }
        // 📍 STUDENT: Get latest location (for map load)
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestLocation(int schoolId, string vehicleNo)
        {
            var latest = await _context.VehicleLiveLocations
                .Where(x => x.SchoolId == schoolId && x.VehicleNo == vehicleNo)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                return NotFound(new
                {
                    message = "No location found"
                });
            }

            return Ok(new
            {
                latitude = latest.Latitude,
                longitude = latest.Longitude,
                speed = latest.Speed,
                updatedAt = latest.UpdatedAt
            });
        }
    }
}