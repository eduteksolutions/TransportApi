using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransportApi.Repository;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodayBirthReportsController : ControllerBase
    {
        private readonly ReportRepository _repository;

        public TodayBirthReportsController(ReportRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var result = await _repository.GetTodayBirthDayList(userId);

            return Ok(result);
        }
    }
}
