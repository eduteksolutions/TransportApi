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

        [HttpPost("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] LiveLocationDto dto)
        {
            if (string.IsNullOrEmpty(dto.VehicleNo))
                return BadRequest("VehicleNo is required");

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

            Console.WriteLine(_context.Entry(entity).State);

            //await _context.SaveChangesAsync();
            var result = await _context.SaveChangesAsync();
            Console.WriteLine(result);
            //Console.WriteLine(_context.Entry(entity).State);
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