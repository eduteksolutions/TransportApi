namespace TransportApi.Services
{
    using Microsoft.Extensions.Hosting;
    using TransportApi.Services;

    public class GpsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public GpsBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var gpsService = scope.ServiceProvider.GetRequiredService<GpsService>();

                    // Example device list (you can load from DB)
                    var devices = new List<string>
                {
                    "860187061953042",
                    "860187062339407"
                };

                    foreach (var device in devices)
                    {
                        await gpsService.SendLiveData(device);
                    }
                }

                await Task.Delay(3000); // every 3 seconds
            }
        }
    }
}
