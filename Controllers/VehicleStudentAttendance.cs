using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleStudentAttendanceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VehicleStudentAttendanceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // INSERT / UPDATE
        [HttpPost]
        public IActionResult Save([FromBody] VehicleStudentAttendance model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "ins_VehicleStudentAttendance",
                    con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AttendanceID", model.AttendanceID);
                cmd.Parameters.AddWithValue("@AdmCd", model.AdmCd);
                cmd.Parameters.AddWithValue("@VehicleID", model.VehicleID);

                cmd.Parameters.AddWithValue("@RouteID",
                    (object?)model.RouteID ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@AttendanceDate",
                    model.AttendanceDate);

                cmd.Parameters.AddWithValue("@PickupTime",
                    (object?)model.PickupTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@DropTime",
                    (object?)model.DropTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@AttendanceStatus",
                    model.AttendanceStatus);

                cmd.Parameters.AddWithValue("@LoginName",
                    model.LoginName);

                cmd.Parameters.AddWithValue("@lUserDt",
                    DateTime.Now);

                cmd.Parameters.AddWithValue("@UserID",
                    model.UserID);

                SqlParameter tranStatus = new SqlParameter(
                    "@Transtatus",
                    SqlDbType.Int);

                tranStatus.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(tranStatus);

                con.Open();

                string msg = "";

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        msg = dr["Msg"].ToString();
                    }
                }

                return Ok(new
                {
                    Transtatus = Convert.ToInt32(tranStatus.Value),
                    Message = msg
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Transtatus = -1,
                    Message = ex.Message
                });
            }
        }

        // GET ALL
        [HttpGet("GetAll")]
        public IActionResult GetAll(int userid)
        {
            List<object> list = new();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                @"SELECT *
                  FROM edu.VehicleStudentAttendance
                  WHERE UserID = @UserID
                  ORDER BY AttendanceDate DESC",
                con);

            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    AttendanceID = dr["AttendanceID"],
                    AdmCd = dr["AdmCd"],
                    VehicleID = dr["VehicleID"],
                    RouteID = dr["RouteID"],
                    AttendanceDate = dr["AttendanceDate"],
                    PickupTime = dr["PickupTime"],
                    DropTime = dr["DropTime"],
                    AttendanceStatus = dr["AttendanceStatus"],
                    UserID = dr["UserID"]
                });
            }

            if (list.Count == 0)
            {
                return NotFound(new
                {
                    Message = "No attendance records found."
                });
            }

            return Ok(list);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                @"SELECT *
                  FROM edu.VehicleStudentAttendance
                  WHERE AttendanceID = @AttendanceID",
                con);

            cmd.Parameters.AddWithValue("@AttendanceID", id);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                return NotFound(new
                {
                    Message = "Attendance record not found."
                });
            }

            return Ok(new
            {
                AttendanceID = dr["AttendanceID"],
                AdmCd = dr["AdmCd"],
                VehicleID = dr["VehicleID"],
                RouteID = dr["RouteID"],
                AttendanceDate = dr["AttendanceDate"],
                PickupTime = dr["PickupTime"],
                DropTime = dr["DropTime"],
                AttendanceStatus = dr["AttendanceStatus"],
                UserID = dr["UserID"]
            });
        }

        // DELETE
        [HttpDelete("{id}/{userid}")]
        public IActionResult Delete(int id, int userid)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                @"DELETE FROM edu.VehicleStudentAttendance
                  WHERE AttendanceID = @AttendanceID
                  AND UserID = @UserID",
                con);

            cmd.Parameters.AddWithValue("@AttendanceID", id);
            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                return NotFound(new
                {
                    Message = "Attendance record not found."
                });
            }

            return Ok(new
            {
                Message = "Deleted Successfully"
            });
        }
    }


}