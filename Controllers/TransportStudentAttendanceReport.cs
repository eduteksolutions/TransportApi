using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportAttendanceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransportAttendanceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/TransportAttendance/TransportAttendanceReport
        [HttpGet("TransportAttendanceReport")]
        public IActionResult TransportAttendanceReport(
     DateTime fromDate,
     DateTime toDate,
     int vCd = 0,
     int routeId = 0,
     int userId = 0)
        {
            try
            {
                List<object> list = new();

                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                using SqlCommand cmd = new SqlCommand("edu.TransportAttendanceRpt", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@VCd", vCd);
                cmd.Parameters.AddWithValue("@RouteID", routeId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                con.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        AttendanceID = dr["AttendanceID"],
                        AdmCd = dr["AdmCd"],
                        StudentName = dr["firstName"].ToString(),
                        VCd = dr["vCd"].ToString(),
                        RouteName = dr["RouteName"].ToString(),
                        AttendanceDate = dr["AttendanceDate"],
                        PickupTime = dr["PickupTime"],
                        DropTime = dr["DropTime"],
                        AttendanceStatus = dr["AttendanceStatus"]
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}