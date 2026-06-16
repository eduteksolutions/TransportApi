using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportRouteController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransportRouteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(int userid)
        {
            List<object> list = new();



            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM edu.TransportRouteMaster  Where UserID=@UserID",
                con);
            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    rCd = dr["rCd"],
                    routeCode = dr["routeCode"],
                    routeName = dr["routeName"],
                    description = dr["description"]

                });
            }

            if (list.Count == 0)
            {
                return NotFound(new
                {
                    message = "No routes found for this user."
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
                "SELECT * FROM edu.TransportRouteMaster WHERE rCd=@rCd",
                con);

            cmd.Parameters.AddWithValue("@rCd", id);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
                return NotFound();

            return Ok(new
            {
                rCd = dr["rCd"],
                routeCode = dr["routeCode"],
                routeName = dr["routeName"],
                description = dr["description"]
            });
        }

        [HttpPost]
        public IActionResult Insert([FromBody] TransportRouteMaster model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "edu.sp_TransportRouteMaster_Insert",
                    con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@routeCode", model.routeCode);
                cmd.Parameters.AddWithValue("@routeName", model.routeName);
                cmd.Parameters.AddWithValue("@description",
                    (object?)model.description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@loginName", model.loginName);
                cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@UserID", model.UserID);

                con.Open();

                using SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                if (dt.Rows.Count > 0)
                {
                    return Ok(new
                    {
                       
                        Message = dt.Rows[0]["Message"].ToString()
                    });
                }

                return Ok(new
                {
                  
                    Message = "No response received from database."
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
        [HttpPut("{id}/{userid}")]
        public IActionResult Update(int id, int userid, [FromBody] TransportRouteMaster model)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "edu.sp_TransportRouteMaster_Update",
                con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@rCd", id);
            cmd.Parameters.AddWithValue("@routeCode", model.routeCode);
            cmd.Parameters.AddWithValue("@routeName", model.routeName);
            cmd.Parameters.AddWithValue("@description", model.description);
            cmd.Parameters.AddWithValue("@loginName", model.loginName);
            cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok($"Updated Successfully by user {userid}");
        }
        [HttpDelete("{id}/{userid}")]
         public IActionResult Delete(int id, int userid)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM edu.TransportRouteMaster WHERE rCd=@rCd and  UserID=@UserID",
                con);

            cmd.Parameters.AddWithValue("@rCd", id);
            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok($"Deleted Successfully by user {userid}");
        }
    }
}
