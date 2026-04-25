using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data.SqlClient;
using System.Data;

using TransportApi.Models;

namespace TransportApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using TransportApi.Data;

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

        [HttpPost("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] LiveLocationDto dto)
        {
            if (string.IsNullOrEmpty(dto.VehicleNo))
                return BadRequest("VehicleNo is required");

            // 🔁 Check if vehicle already exists
            var existing = await _context.VehicleLiveLocations
                .FirstOrDefaultAsync(x => x.VehicleNo == dto.VehicleNo);

            if (existing != null)
            {
                // UPDATE existing record
                existing.Latitude = dto.Latitude;
                existing.Longitude = dto.Longitude;
                existing.Speed = dto.Speed;
                existing.UpdatedAt = DateTime.Now;
            }
            else
            {
                // INSERT new record
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
            }

            await _context.SaveChangesAsync();

            // 📡 REAL-TIME PUSH (SignalR)
            await _hub.Clients.All.SendAsync("ReceiveLocation", dto);

            return Ok(new
            {
                message = "Live location updated",
                data = dto
            });
        }
    }
}