using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportAdmissionController : ControllerBase
    {
        private readonly IConfiguration _configuration;


    public TransportAdmissionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ==================================================
        // UPDATE VEHICLE NUMBER
        // PUT:
        // api/TransportAdmission/UpdateVehicleNo/183/9026/1/HR05AB1234
        // ==================================================
       

        [HttpPut("UpdateStations")]
        public IActionResult UpdateStations([FromBody] UpdateStationRequest model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                using SqlCommand cmd = new SqlCommand("edu.sp_UpdateTransportAdmissionStations", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserID", model.UserId);
                cmd.Parameters.AddWithValue("@Code", model.Code);
                cmd.Parameters.AddWithValue("@AdmCd", model.AdmCd);
                cmd.Parameters.AddWithValue("@PickupStationCd", model.PickupStationCd);
                cmd.Parameters.AddWithValue("@DropStationCd", model.DropStationCd);

                con.Open();

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int status = Convert.ToInt32(reader["Status"]);
                    string message = reader["Message"].ToString();

                    if (status == -1)
                        return Unauthorized(new { Message = message });

                    if (status == 0)
                        return NotFound(new { Message = message });

                    return Ok(new
                    {
                        Status = true,
                        Message = message
                    });
                }

                return StatusCode(500, new { Message = "Unexpected database response." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        // =======================a===========================
        // GET STUDENTS BY VEHICLE NUMBER
        // GET:
        // api/TransportAdmission/GetTransportStudentsByVehicle
        // ?userId=9026&teacherCode=1&vehicleNo=HR05AB1234
        // ==================================================


        [HttpGet("GetTransportStudentsByVehicle")]
        public IActionResult GetTransportStudentsByVehicle(
            int userId,
            int teacherCode,
            string vehicleNo)
        {
            try
            {
                List<object> students = new();

                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                con.Open();

                // Check View Rights
                SqlCommand rightsCmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM HRDStaffMasterAccess
                WHERE UserID = @UserID
                  AND Code = @TeacherCode
                  AND FormName = 'Transport Vehicle Assignment'
                  AND CanView = 'Y'", con);

                rightsCmd.Parameters.AddWithValue("@UserID", userId);
                rightsCmd.Parameters.AddWithValue("@TeacherCode", teacherCode);

                int hasRights = Convert.ToInt32(
                    rightsCmd.ExecuteScalar());

                if (hasRights == 0)
                {
                    return Unauthorized(new
                    {
                        Message = "You do not have permission to view students."
                    });
                }

                SqlCommand cmd = new SqlCommand(
                    "[edu].[GetTransportStudentsByVehicle]",
                    con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@VehicleNo", vehicleNo);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    students.Add(new
                    {
                        AdmCd = dr["AdmCd"],
                        Name = dr["Name"],
                        AdmCode = dr["AdmCode"],
                        VehicleNo = dr["VehicleNo"],
                        ClassName = dr["ClassName"],
                        SecName = dr["SecName"],
                        RollNo = dr["RollNo"],
                        FatherName = dr["FName"],
                        ContactNo = dr["ContactNo"],
                        Address = dr["Address"],
                        Pic = dr["Pic"],
                        RegNo = dr["RegNo"]
                    });
                }

                if (students.Count == 0)
                {
                    return NotFound(new
                    {
                        Message = "No students found."
                    });
                }

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }
    }


}
