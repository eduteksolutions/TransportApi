using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportApi.Data;
using TransportApi.Models;
using System.Linq;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/student-location")]
    public class StudentLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentLocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📍 SAVE OR UPDATE STUDENT LOCATION
        [HttpPost("save")]
        public async Task<IActionResult> SaveStudentLocation(
            [FromBody] StudentLocationDto dto)
        {
            try
            {
                // Validate Admission Code
                if (string.IsNullOrEmpty(dto.AdmCd))
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Admission code required"
                    });
                }

                // Validate Coordinates
                if (dto.Latitude < -90 || dto.Latitude > 90 ||
                    dto.Longitude < -180 || dto.Longitude > 180)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Invalid coordinates"
                    });
                }

                // Check existing student location
                var existingLocation = await _context.StudentLocations
                    .FirstOrDefaultAsync(x => x.AdmCd == dto.AdmCd);

                // UPDATE EXISTING LOCATION
                if (existingLocation != null)
                {
                    existingLocation.UserId = dto.UserId;
                    existingLocation.Latitude = dto.Latitude;
                    existingLocation.Longitude = dto.Longitude;
                    existingLocation.CreatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        status = true,
                        message = "Student location updated successfully",
                        data = existingLocation
                    });
                }

                // SAVE NEW LOCATION
                var entity = new StudentLocation
                {
                    AdmCd = dto.AdmCd,
                    UserId = dto.UserId,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    CreatedAt = DateTime.Now
                };

                _context.StudentLocations.Add(entity);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = true,
                    message = "Student location saved successfully",
                    data = entity
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        // 📍 GET LATEST LOCATION
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestLocation(string admCd)
        {
            try
            {
                if (string.IsNullOrEmpty(admCd))
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Admission code required"
                    });
                }

                var latest = await _context.StudentLocations
                    .Where(x => x.AdmCd == admCd)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (latest == null)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "No location found"
                    });
                }

                return Ok(new
                {
                    status = true,
                    message = "Location fetched successfully",
                    data = new
                    {
                        admCd = latest.AdmCd,
                        userId = latest.UserId,
                        latitude = latest.Latitude,
                        longitude = latest.Longitude,
                        createdAt = latest.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
    }

    // DTO
    public class StudentLocationDto
    {
        public string AdmCd { get; set; } = "";
        public string UserId { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}