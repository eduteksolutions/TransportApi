using System.Data;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Google.Apis.Auth.OAuth2;
using TransportApi.Services;



namespace TransportApi.Controllers
{
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        // Directly injecting the concrete service class
        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("api/firebase/token")]
        public async Task<IActionResult> GetAccessToken()
        {
            var token = await _notificationService.GetAccessTokenAsync();
            return Ok(new { token });
        }

        [HttpGet]
        [Route("api/send-notification")]
        public async Task<IActionResult> SendNotification(string deviceToken, string title, string body)
        {
            if (string.IsNullOrEmpty(deviceToken))
                return BadRequest("Missing device token.");

            var result = await _notificationService.SendMessageAsync(deviceToken, title, body);
            return Ok(new { result });
        }
    }
}
