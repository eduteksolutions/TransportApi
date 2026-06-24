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

        public TransportVehicleRouteDetailsController(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/TransportVehicleRouteDetails/GetAll?userid=1
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
                    FROM TransportVehicleRouteDetails
                    WHERE UserID = @UserID
                    ORDER BY SerialNo",
                    con);

                cmd.Parameters.AddWithValue("@UserID", userid);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        Id = dr["Id"],
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

        // GET: api/TransportVehicleRouteDetails/1/1
        [HttpGet("{id}/{userid}")]
        public IActionResult GetById(int id, int userid)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(@"
                    SELECT *
                    FROM VehicleRouteDetails
                    WHERE Id=@Id
                    AND UserID=@UserID",
                    con);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserID", userid);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    return NotFound(new
                    {
                        Message = "Record not found."
                    });
                }

                return Ok(new
                {
                    Id = dr["Id"],
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
                    UserID = dr["UserID"]
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

        // POST: api/TransportVehicleRouteDetails
        [HttpPost]
        public IActionResult Insert(
            [FromBody] TransportVehicleRouteDetail model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "InsertVehicleRouteDetails",
                    con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@VehicleCode", model.VehicleCode);
                cmd.Parameters.AddWithValue("@SerialNo", model.SerialNo);
                cmd.Parameters.AddWithValue("@VehicleNo", model.VehicleNo);
                cmd.Parameters.AddWithValue("@RouteCode", model.RouteCode);
                cmd.Parameters.AddWithValue("@DriverCode", model.DriverCode);
                cmd.Parameters.AddWithValue("@CoDriverCode", model.CoDriverCode);
                cmd.Parameters.AddWithValue("@ValidDate", model.ValidDate);
                cmd.Parameters.AddWithValue("@MorningTime", model.MorningTime);
                cmd.Parameters.AddWithValue("@EveningTime", model.EveningTime);
                cmd.Parameters.AddWithValue("@LoginName", model.LoginName);
                cmd.Parameters.AddWithValue("@UserID", model.UserID);

                SqlParameter transStatus =
                    new SqlParameter("@TransStatus", SqlDbType.Int);

                transStatus.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(transStatus);

                con.Open();

                cmd.ExecuteNonQuery();

                int result =
                    Convert.ToInt32(transStatus.Value);

                if (result == 2)
                {
                    return BadRequest(new
                    {
                        Message = "Duplicate Record."
                    });
                }

                return Ok(new
                {
                    Message = "Record saved successfully."
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

        // PUT: api/TransportVehicleRouteDetails/1/1
        [HttpPut("{id}/{userid}")]
        public IActionResult Update(
            int id,
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
                        VehicleCode=@VehicleCode,
                        SerialNo=@SerialNo,
                        VehicleNo=@VehicleNo,
                        RouteCode=@RouteCode,
                        DriverCode=@DriverCode,
                        CoDriverCode=@CoDriverCode,
                        ValidDate=@ValidDate,
                        MorningTime=@MorningTime,
                        EveningTime=@EveningTime,
                        LoginName=@LoginName
                    WHERE Id=@Id
                    AND UserID=@UserID",
                    con);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserID", userid);

                cmd.Parameters.AddWithValue("@VehicleCode", model.VehicleCode);
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

        // DELETE: api/TransportVehicleRouteDetails/1/1
        [HttpDelete("{id}/{userid}")]
        public IActionResult Delete(
            int id,
            int userid)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(@"
                    DELETE FROM VehicleRouteDetails
                    WHERE Id=@Id
                    AND UserID=@UserID",
                    con);

                cmd.Parameters.AddWithValue("@Id", id);
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