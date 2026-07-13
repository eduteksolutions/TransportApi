using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TransportApi.Data;

namespace TransportApi.Controllers
{

  
        [ApiController]
        [Route("api/busstation")]
        public class BusStationNotificationController : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            private readonly IHubContext<DataHub> _hub;

            public BusStationNotificationController(
                ApplicationDbContext context,
                IHubContext<DataHub> hub)
            {
                _context = context;
                _hub = hub;
            }

            // =====================================
            // CHECK BUS DISTANCE FROM STATION
            // =====================================
            [HttpPost("check")]
            public async Task<IActionResult> CheckBusStation(
                int schoolId,
                string deviceId,
                double latitude,
                double longitude)
            {
                try
                {
                // 1. Join Admission to TransportStationMaster using PickupStationCd -> StCode
                // 1. Join Admission to TransportStationMaster using clean Query Syntax
                // 1. Fetch the students and link them to their stations using a simple cross-where clause
                // 1. Join Admission to TransportStationMaster using Query Syntax
                var students = await (from student in _context.Admission
                                      from station in _context.TransportStationMaster
                                      where student.UserID == schoolId
                                        && student.PickupStationCd == station.StCode
                                        && student.UserID == station.UserID
                                      select new
                                      {
                                          student.AdmCd,
                                          StudentName = student.FirstName,
                                          StationName = station.StName,
                                          Latitude = station.Latitude,   // <-- MUST INCLUDE THIS HERE
                                          Longitude = station.Longitude // <-- MUST INCLUDE THIS HERE
                                      })
                                      .ToListAsync();

                // 2. Process geofencing for each student station
                foreach (var student in students)
                {
                    // Now student.Latitude and student.Longitude will exist perfectly!
                    if (string.IsNullOrWhiteSpace(student.Latitude.ToString()) || string.IsNullOrWhiteSpace(student.Longitude.ToString()))
                        continue;

                    if (!double.TryParse(student.Latitude.ToString(), out double stationLat) ||
                        !double.TryParse(student.Longitude.ToString(), out double stationLng))
                    {
                        continue;
                    }

                    double distance = CalculateDistance(
                        latitude,
                        longitude,
                        stationLat,
                        stationLng
                    );

                    // 3. Trigger Real-time SignalR notifications if bus is within 100 meters
                    if (distance <= 100)
                    {
                        await _hub.Clients
                            .Group($"Student_{student.AdmCd}")
                            .SendAsync("StudentNotification", new
                            {
                                title = "Bus Arriving",
                                message = $"Bus is {Math.Round(distance)} meters from {student.StationName}",
                                admCd = student.AdmCd,
                                station = student.StationName,
                                deviceId = deviceId
                            });
                    }
                }


                return Ok(new
                    {
                        success = true,
                        message = "Station check completed"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        error = ex.Message
                    });
                }
            }

            // =====================================
            // DISTANCE CALCULATION (Haversine Formula)
            // =====================================
            private double CalculateDistance(
                double lat1,
                double lon1,
                double lat2,
                double lon2)
            {
                const double R = 6371000; // Earth's radius in meters

                double dLat = (lat2 - lat1) * Math.PI / 180;
                double dLon = (lon2 - lon1) * Math.PI / 180;

                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                         + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                         * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                return R * c;
            }
        }
    }


