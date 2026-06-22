using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TransportApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;

    [ApiController]
    [Route("api/[controller]")]
    public class HRDStaffController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HRDStaffController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/HRDStaff/GetDrivers?userid=1
        [HttpGet("GetDrivers")]
        public IActionResult GetDrivers(int userid)
        {
            List<object> list = new();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(@"
            SELECT Code, SName
            FROM HRDStaffMaster
            WHERE Designation = 13
            AND UserID = @UserID
            ORDER BY SName", con);

            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    Code = dr["Code"],
                    Name = dr["SName"]
                });
            }

            return Ok(list);
        }

        // GET: api/HRDStaff/GetCoDrivers?userid=1
        [HttpGet("GetCoDrivers")]
        public IActionResult GetCoDrivers(int userid)
        {
            List<object> list = new();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(@"
            SELECT Code, SName
            FROM HRDStaffMaster
            WHERE Designation = 10
            AND UserID = @UserID
            ORDER BY SName", con);

            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    Code = dr["Code"],
                    Name = dr["SName"]
                });
            }

            return Ok(list);
        }

        // GET: api/HRDStaff/GetAll?userid=1
        [HttpGet("GetAll")]
        public IActionResult GetAll(int userid)
        {
            List<object> list = new();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(@"
            SELECT *
            FROM HRDStaffMaster
            WHERE UserID = @UserID
            ORDER BY SName", con);

            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    Code = dr["Code"],
                    SName = dr["SName"],
                    Designation = dr["Designation"]
                });
            }

            return Ok(list);
        }
    }
}
