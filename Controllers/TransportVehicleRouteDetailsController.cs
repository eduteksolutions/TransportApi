using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportVehicleRouteDetailsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransportVehicleRouteDetailsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ==========================================
        // GET ALL
        // GET: api/TransportVehicleRouteDetails/GetAll?userid=9026
        // ==========================================
        [HttpGet("GetAll")]
        public IActionResult GetAll(int userid)
        {
            try
            {
                List<object> list = new();

                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(@"
                    SELECT *
                    FROM VehicleRouteDetails
                    WHERE UserID = @UserID
                    ORDER BY SerialNo", con);

                cmd.Parameters.AddWithValue("@UserID", userid);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        VehicleCode = dr["VehicleCode"],
                        SerialNo = dr["SerialNo"],
                        VehicleNo = dr["VehicleNo"],
                        RouteCode = dr["RouteCode"],
                        DriverCode = dr["DriverCode"],
                        CoDriverCode = dr["CoDriverCode"],
                        ValidDate = dr["ValidDate"],
                        MorningTime = dr["MorningTime"],
                        EveningTime = dr["EveningTime"],
                        LoginName = dr["LoginName"],
                        LoggedDate = dr["LoggedDate"],
                        UserID = dr["UserID"]
                    });
                }

                if (list.Count == 0)
                {
                    return NotFound(new
                    {
                        Message = "No records found."
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }

        // ==========================================
        // UPDATE
        // PUT: api/TransportVehicleRouteDetails/4/9026
        // ==========================================
        [HttpPut("{vehicleCode}/{userid}")]
        public IActionResult Update(
            int vehicleCode,
            int userid,
            [FromBody] TransportVehicleRouteDetail model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(@"
                    UPDATE VehicleRouteDetails
                    SET
                        SerialNo = @SerialNo,
                        VehicleNo = @VehicleNo,
                        RouteCode = @RouteCode,
                        DriverCode = @DriverCode,
                        CoDriverCode = @CoDriverCode,
                        ValidDate = @ValidDate,
                        MorningTime = @MorningTime,
                        EveningTime = @EveningTime,
                        LoginName = @LoginName
                    WHERE VehicleCode = @VehicleCode
                    AND UserID = @UserID", con);

                cmd.Parameters.AddWithValue("@VehicleCode", vehicleCode);
                cmd.Parameters.AddWithValue("@UserID", userid);

                cmd.Parameters.AddWithValue("@SerialNo", model.SerialNo);
                cmd.Parameters.AddWithValue("@VehicleNo", model.VehicleNo);
                cmd.Parameters.AddWithValue("@RouteCode", model.RouteCode);
                cmd.Parameters.AddWithValue("@DriverCode", model.DriverCode);
                cmd.Parameters.AddWithValue("@CoDriverCode", model.CoDriverCode);
                cmd.Parameters.AddWithValue("@ValidDate", model.ValidDate);
                cmd.Parameters.AddWithValue("@MorningTime", model.MorningTime);
                cmd.Parameters.AddWithValue("@EveningTime", model.EveningTime);
                cmd.Parameters.AddWithValue("@LoginName", model.LoginName);

                con.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    return NotFound(new
                    {
                        Message = "Record not found."
                    });
                }

                return Ok(new
                {
                    Message = "Updated successfully."
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

        // ==========================================
        // DELETE
        // DELETE: api/TransportVehicleRouteDetails/4/9026
        // ==========================================
        [HttpDelete("{vehicleCode}/{userid}")]
        public IActionResult Delete(
            int vehicleCode,
            int userid)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(@"
                    DELETE FROM VehicleRouteDetails
                    WHERE VehicleCode = @VehicleCode
                    AND UserID = @UserID", con);

                cmd.Parameters.AddWithValue("@VehicleCode", vehicleCode);
                cmd.Parameters.AddWithValue("@UserID", userid);

                con.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    return NotFound(new
                    {
                        Message = "Record not found."
                    });
                }

                return Ok(new
                {
                    Message = "Deleted successfully."
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
    }
}