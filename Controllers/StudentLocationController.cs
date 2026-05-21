using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransportApi.Data;
using TransportApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TransportApi.Controllers
        {
            [ApiController]
            [Route("api/student-location")]
            public class StudentLocationController : ControllerBase
            {
                private readonly ApplicationDbContext _context;

                public StudentLocationController(
                    ApplicationDbContext context)
                {
                    _context = context;
                }

                // 📍 SAVE STUDENT LOCATION
                [HttpPost("save")]
                public async Task<IActionResult> SaveStudentLocation(
                    [FromBody] StudentLocationDto dto)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(dto.AdmCd))
                        {
                            return BadRequest(new
                            {
                                message = "Admission code required"
                            });
                        }

                        if (dto.Latitude < -90 || dto.Latitude > 90 ||
                            dto.Longitude < -180 || dto.Longitude > 180)
                        {
                            return BadRequest(new
                            {
                                message = "Invalid coordinates"
                            });
                        }

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
                            message = "Student location saved",
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
                public async Task<IActionResult> GetLatestLocation(
                    string admCd)
                {
                    var latest = await _context.StudentLocations
                        .Where(x => x.AdmCd == admCd)
                        .OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (latest == null)
                    {
                        return NotFound(new
                        {
                            message = "No location found"
                        });
                    }

                    return Ok(new
                    {
                        admCd = latest.AdmCd,
                        userId = latest.UserId,
                        latitude = latest.Latitude,
                        longitude = latest.Longitude,
                        createdAt = latest.CreatedAt
                    });
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
   

