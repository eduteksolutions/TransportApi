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
            string vehicleno ,
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
                cmd.Parameters.AddWithValue("@VehicleNo", vehicleno);
                cmd.Parameters.AddWithValue("@RouteID", routeId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                con.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        AttendanceID = dr["AttendanceID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AttendanceID"]),
                        AdmCd = dr["AdmCd"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AdmCd"]),
                        StudentName = dr["StudentName"]?.ToString(),
                        VehicleCode = dr["vCd"] == DBNull.Value ? 0 : Convert.ToInt32(dr["vCd"]),
                        VehicleNo = dr["VehicleNo"]?.ToString(),
                        RouteName = dr["RouteName"]?.ToString(),
                        AttendanceDate = dr["AttendanceDate"] == DBNull.Value
                            ? ""
                            : Convert.ToDateTime(dr["AttendanceDate"]).ToString("yyyy-MM-dd"),
                        PickupTime = dr["PickupTime"] == DBNull.Value
                            ? ""
                            : Convert.ToDateTime(dr["PickupTime"]).ToString("hh:mm tt"),
                        DropTime = dr["DropTime"] == DBNull.Value
                            ? ""
                            : Convert.ToDateTime(dr["DropTime"]).ToString("hh:mm tt"),
                        AttendanceStatus = dr["AttendanceStatus"]?.ToString(),
                        LoginName = dr["LoginName"]?.ToString(),
                        LoggedDate = dr["lUserDt"] == DBNull.Value
                            ? ""
                            : Convert.ToDateTime(dr["lUserDt"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        Message = dr["Message"]?.ToString()
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }
    }
}