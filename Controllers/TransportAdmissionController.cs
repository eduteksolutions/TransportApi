using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

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
        [HttpPut("UpdateStation/{admCd}/{userId}/{Code}/{PickupStationCd}/{DropStationCd}")]
        public IActionResult UpdateStation(
            int admCd, 
            int userId,
            int Code,
            int PickupStationCd,int DropStationCd)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                con.Open();

                // Check Edit Rights
                SqlCommand rightsCmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM HRDStaffMasterAccess
                WHERE UserID = @UserID
                  AND Code = @Code
                  AND FormName = 'Transport Vehicle Assignment'
                  AND CanEdit = 'Y'", con);

                rightsCmd.Parameters.AddWithValue("@UserID", userId);
                rightsCmd.Parameters.AddWithValue("@Code", Code);

                int hasRights = Convert.ToInt32(
                    rightsCmd.ExecuteScalar());

                if (hasRights == 0)
                {
                    return Unauthorized(new
                    {
                        Message = "You do not have permission to update vehicle number."
                    });
                }

                SqlCommand cmd = new SqlCommand(@"
                UPDATE Admission
                SET PickupStationCd = @PickupStationCd,trans='Y',DropStationCd=@DropStationCd
                WHERE AdmCd = @AdmCd
                  AND UserID = @UserID", con);


                cmd.Parameters.AddWithValue("@PickupStationCd", @PickupStationCd);
                cmd.Parameters.AddWithValue("@DropStationCd", @DropStationCd);


                cmd.Parameters.AddWithValue("@AdmCd", admCd);
                cmd.Parameters.AddWithValue("@UserID", userId);

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    return NotFound(new
                    {
                        Message = "Admission record not found."
                    });
                }

                return Ok(new
                {
                    


                    Status = true,
                    Message = "Station updated successfully.",
                    AdmCd = admCd,
                    UserID = userId,
                    TeacherCode = Code,
                    PickupStationCd = PickupStationCd,
                    DropStationCd = DropStationCd
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
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
