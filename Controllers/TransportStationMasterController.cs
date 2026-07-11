using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;


namespace TransportApi.Controllers
{
 
        [Route("api/[controller]")]
        [ApiController]
        public class TransportStationMasterController : ControllerBase
        {
            private readonly IConfiguration _configuration;

            public TransportStationMasterController(IConfiguration configuration)
            {
                _configuration = configuration;
            }
        [HttpPut("{id}/{userid}")]
        public IActionResult Update(int id, int userid, [FromBody] TransportStationMaster model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "dbo.ins_TransportStationMaster",
                    con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StCode", id);
                cmd.Parameters.AddWithValue("@StName", model.StName);
                cmd.Parameters.AddWithValue("@SubRtCd", model.SubRtCd);
                cmd.Parameters.AddWithValue("@LoginName", model.LoginName);
                cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now);

                cmd.Parameters.AddWithValue("@Latitude",
                    (object?)model.Latitude ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Longitude",
                    (object?)model.Longitude ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@ArrivalTime",
                    (object?)model.ArrivalTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@DepartureTime",
                    (object?)model.DepartureTime ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@UserID", userid);

                cmd.Parameters.AddWithValue("@startLat",
                    (object?)model.startLat ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@startLng",
                    (object?)model.startLng ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@endLat",
                    (object?)model.endLat ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@endLng",
                    (object?)model.endLng ?? DBNull.Value);

                SqlParameter tranStatus = new SqlParameter("@Transtatus", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(tranStatus);

                con.Open();

                string message = "";

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    message = reader["Msg"].ToString();
                }

                return Ok(new
                {
                    Status = Convert.ToInt32(tranStatus.Value),
                    Message = message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = -1,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("GetBySubStation/{subRtCd}/{userid}")]
        public IActionResult GetBySubStation(string subRtCd, int userid)
        {
            List<object> list = new();

            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                // Filters by both the Sub-Station/Sub-Route code and the User ID
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM TransportStationMaster WHERE SubRtCd = @SubRtCd AND UserID = @UserID",
                    con);

                cmd.Parameters.AddWithValue("@SubRtCd", subRtCd);
                cmd.Parameters.AddWithValue("@UserID", userid);

                con.Open();

                using SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        StCode = dr["StCode"],
                        StName = dr["StName"],
                        SubRtCd = dr["SubRtCd"],
                        Latitude = dr["Latitude"],
                        Longitude = dr["Longitude"],
                        ArrivalTime = dr["ArrivalTime"],
                        DepartureTime = dr["DepartureTime"],
                        startLat = dr["startLat"],
                        startLng = dr["startLng"],
                        endLat = dr["endLat"],
                        endLng = dr["endLng"],
                        UserID = dr["UserID"]
                    });
                }

                if (list.Count == 0)
                {
                    return NotFound(new
                    {
                        Message = $"No stations found for Sub-Station: {subRtCd} under this user."
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = -1,
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
            public IActionResult Save([FromBody] TransportStationMaster model)
            {
                try
                {
                    using SqlConnection con = new SqlConnection(
                        _configuration.GetConnectionString("DefaultConnection"));

                    SqlCommand cmd = new SqlCommand(
                        "dbo.ins_TransportStationMaster",
                        con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StCode", model.StCode);
                    cmd.Parameters.AddWithValue("@StName", model.StName);
                    cmd.Parameters.AddWithValue("@SubRtCd", model.SubRtCd);
                    cmd.Parameters.AddWithValue("@LoginName", model.LoginName);
                    cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now);

                    cmd.Parameters.AddWithValue("@Latitude",
                        (object?)model.Latitude ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@Longitude",
                        (object?)model.Longitude ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@ArrivalTime",
                        (object?)model.ArrivalTime ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@DepartureTime",
                        (object?)model.DepartureTime ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@UserID", model.UserID);

                    cmd.Parameters.AddWithValue("@startLat",
                        (object?)model.startLat ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@startLng",
                        (object?)model.startLng ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@endLat",
                        (object?)model.endLat ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@endLng",
                        (object?)model.endLng ?? DBNull.Value);

                    SqlParameter tranStatus = new SqlParameter("@Transtatus", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(tranStatus);

                    con.Open();

                    string message = "";

                    using SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        message = reader["Msg"].ToString();
                    }

                    return Ok(new
                    {
                        Status = Convert.ToInt32(tranStatus.Value),
                        Message = message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Status = -1,
                        Message = ex.Message
                    });
                }
            }

            [HttpGet("GetAll")]
            public IActionResult GetAll(int userid)
            {
                List<object> list = new();

                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM TransportStationMaster WHERE UserID=@UserID",
                    con);

                cmd.Parameters.AddWithValue("@UserID", userid);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        StCode = dr["StCode"],
                        StName = dr["StName"],
                        SubRtCd = dr["SubRtCd"],
                        Latitude = dr["Latitude"],
                        Longitude = dr["Longitude"],
                        ArrivalTime = dr["ArrivalTime"],
                        DepartureTime = dr["DepartureTime"],
                        UserID = dr["UserID"]
                    });
                }

                if (list.Count == 0)
                {
                    return NotFound(new
                    {
                        Message = "No stations found for this user."
                    });
                }

                return Ok(list);
            }

            [HttpGet("{id}")]
            public IActionResult GetById(int id)
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM TransportStationMaster WHERE StCode=@StCode",
                    con);

                cmd.Parameters.AddWithValue("@StCode", id);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                    return NotFound();

                return Ok(new
                {
                    StCode = dr["StCode"],
                    StName = dr["StName"],
                    SubRtCd = dr["SubRtCd"],
                    Latitude = dr["Latitude"],
                    Longitude = dr["Longitude"],
                    ArrivalTime = dr["ArrivalTime"],
                    DepartureTime = dr["DepartureTime"],
                    startLat = dr["startLat"],
                    startLng = dr["startLng"],
                    endLat = dr["endLat"],
                    endLng = dr["endLng"],
                    UserID = dr["UserID"]
                });
            }

            [HttpDelete("{id}/{userid}")]
            public IActionResult Delete(int id, int userid)
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM TransportStationMaster WHERE StCode=@StCode AND UserID=@UserID",
                    con);

                cmd.Parameters.AddWithValue("@StCode", id);
                cmd.Parameters.AddWithValue("@UserID", userid);

                con.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    return NotFound(new
                    {
                        Message = "Station not found."
                    });
                }

                return Ok(new
                {
                    Message = $"Deleted Successfully by user {userid}"
                });
            }
        }
    }

