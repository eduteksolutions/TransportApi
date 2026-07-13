using Microsoft.AspNetCore.Mvc;
using TransportApi.Models;
using TransportApi.Services;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly BusProximityService _proximityService;

        // Removed IHubContext<DataHub> dependency entirely to prevent startup/null crashes
        public TestController(BusProximityService proximityService)
        {
            _proximityService = proximityService;
        }

        [HttpPost("send-fake-location")]
        public async Task<IActionResult> SendFakeLocation([FromBody] FakeLocationRequest request)
        {
            try
            {
                var deviceIdClean = request.DeviceId?.Trim().ToUpper() ?? "UNKNOWN";

                // Trigger the proximity logic pipeline directly
                await _proximityService.CheckAndNotifyAsync(
                    schoolId: request.SchoolId,
                    deviceId: deviceIdClean,
                    busLat: request.Latitude,
                    busLng: request.Longitude
                );

                return Ok(new { status = "Success", message = "Proximity service pipeline triggered successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Error", message = ex.Message });
            }
        }
    }

    public class FakeLocationRequest
    {
        public int SchoolId { get; set; }
        public string DeviceId { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double Course { get; set; }
    }
}