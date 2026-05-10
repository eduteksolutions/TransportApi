using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    // Controllers/TestController.cs
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IHubContext<DataHub> _hub;

        public TestController(IHubContext<DataHub> hub)
        {
            _hub = hub;
        }

        [HttpPost("send-fake-location")]
        public async Task<IActionResult> SendFakeLocation(
            [FromBody] FakeLocationRequest request)
        {
            var dto = new DeviceLocationDto
            {
                SchoolId = request.SchoolId,
                DeviceId = request.DeviceId.Trim().ToUpper(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Speed = request.Speed,
                Course = request.Course,
            };

            var group = $"{request.SchoolId}_{dto.DeviceId}";

            await _hub.Clients
                .Group(group)
                .SendAsync("ReceiveLocation", dto);

            return Ok(new
            {
                message = "Location sent",
                group,
                dto
            });
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
