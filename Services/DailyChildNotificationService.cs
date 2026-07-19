using Microsoft.Extensions.Hosting;
using TransportApi.Repository;

namespace TransportApi.Services
{
   
    public class DailyChildNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailyChildNotificationService> _logger;

        public DailyChildNotificationService(
            IServiceScopeFactory scopeFactory,
            ILogger<DailyChildNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                var nextRun = DateTime.Today.AddHours(6); // 8 AM

                if (now >= nextRun)
                {
                    nextRun = nextRun.AddDays(1);
                }

                await Task.Delay(nextRun - now, stoppingToken);

                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider
                    .GetRequiredService<ReportRepository>();

                var notificationService = scope.ServiceProvider
                    .GetRequiredService<INotificationService>();

                // Get all children having device token
                var children = await repository.GetAllChildrenWithDeviceId();

                foreach (var child in children)
                {
                    if (!string.IsNullOrEmpty(child.mbl_Device_ID))
                    {
                        await notificationService.SendMessageAsync(
     child.mbl_Device_ID,
     "🌞 Good Morning!",
     "Have a wonderful day ahead. Stay happy and keep learning!zioSchool"
 );
                    }
                }

                _logger.LogInformation("Daily child notifications sent.");
            }
        }
    
    
    }
}
