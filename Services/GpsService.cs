using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;

namespace TransportApi.Services
{
    public class GpsService
    {
        private readonly IHubContext<DataHub> _hub;
        private readonly IHttpClientFactory _httpClientFactory;

        public GpsService(IHubContext<DataHub> hub, IHttpClientFactory httpClientFactory)
        {
            _hub = hub;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendLiveData(string deviceId)
        {
            var client = _httpClientFactory.CreateClient();

            var url = "https://mvts1.millitrack.com/api/middleMan/getDeviceInfo?accessToken=YOUR_TOKEN";

            var res = await client.GetStringAsync(url);

            var json = JObject.Parse(res);

            var devices = json["object"] as JArray;
            if (devices == null) return;

            var device = devices.FirstOrDefault(x =>
                x["deviceUniqueId"]?.ToString() == deviceId);

            if (device == null) return;

            var data = new
            {
                deviceId = device["deviceUniqueId"]?.ToString(),
                latitude = device["latitude"]?.ToObject<double>(),
                longitude = device["longitude"]?.ToObject<double>(),
                altitude = device["altitude"]?.ToObject<double>(),
                speed = device["speed"]?.ToObject<double>(),
                course = device["course"]?.ToObject<double>(),
                ignition = device["attributes"]?["ignition"]?.ToObject<bool>(),
                battery = device["attributes"]?["batteryLevel"]?.ToObject<double>(),
                name = device["name"]?.ToString()
            };

            await _hub.Clients
                .Group(deviceId.Trim().ToUpper())
                .SendAsync("ReceiveLocation", data);
        }
    }
}