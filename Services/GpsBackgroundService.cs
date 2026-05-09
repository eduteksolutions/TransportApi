using Microsoft.EntityFrameworkCore;
using TransportApi.Data;

namespace TransportApi.Services
{
    public class GpsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public GpsBackgroundService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope =
                        _serviceProvider.CreateScope();

                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    var gpsService = scope.ServiceProvider
                        .GetRequiredService<GpsService>();

                    // =========================
                    // GET ACTIVE SCHOOLS
                    // =========================
                    var schoolIds = await context
                        .SchoolGpsSettings
                        .Where(x => x.IsActive)
                        .Select(x => x.SchoolId)
                        .Distinct()
                        .ToListAsync();

                    // =========================
                    // LOOP SCHOOLS
                    // =========================
                    foreach (var schoolId in schoolIds)
                    {
                        await gpsService
                            .SendSchoolLiveData(schoolId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // EVERY 5 SECONDS
                await Task.Delay(
                    5000,
                    stoppingToken);
            }
        }
    }
}