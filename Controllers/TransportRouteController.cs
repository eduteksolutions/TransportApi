using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RouteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = new List<object>();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM edu.TransportRouteMaster",
                con);

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
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "edu.sp_TransportRouteMaster_Insert",
                con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@rCd", model.rCd);
            cmd.Parameters.AddWithValue("@routeCode", model.routeCode);
            cmd.Parameters.AddWithValue("@routeName", model.routeName);
            cmd.Parameters.AddWithValue("@description", model.description);
            cmd.Parameters.AddWithValue("@loginName", model.loginName);
            cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@UserID", model.UserID);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(dr);

            return Ok(dt);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TransportRouteMaster model)
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
            cmd.Parameters.AddWithValue("@UserID", model.UserID);

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok("Updated Successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM edu.TransportRouteMaster WHERE rCd=@rCd",
                con);

            cmd.Parameters.AddWithValue("@rCd", id);

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok("Deleted Successfully");
        }

    }
}
