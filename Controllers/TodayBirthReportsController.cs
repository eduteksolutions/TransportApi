using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportApi.Repository;
using TransportApi.Services;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodayBirthReportsController : ControllerBase
    {
        private readonly ReportRepository _repository;
        private readonly INotificationService _notificationService;

        public TodayBirthReportsController(
            ReportRepository repository,
            INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Get today's birthday students
                var students = await _repository.GetTodayBirthDayList();


                if (students == null || students.Count == 0)
                {
                    return Ok(new
                    {
                        Status = false,
                        Message = "No birthday students found today."
                    });
                }


                int notificationCount = 0;


                foreach (var student in students)
                {
                    if (!string.IsNullOrEmpty(student.mbl_Device_ID))
                    {
                        var response =
                            await _notificationService.SendMessageAsync(
                                student.mbl_Device_ID,
                                "🎂 Happy Birthday",
                                $"Happy Birthday {student.Name}! Have a wonderful day."
                            );

                        notificationCount++;
                    }
                }


                return Ok(new
                {
                    Status = true,
                    TotalStudents = students.Count,
                    NotificationsSent = notificationCount,
                    Data = students
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
    }
}