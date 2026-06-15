using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;

namespace TransportApi.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using System.Data;

    [Route("api/[controller]")]
    [ApiController]
    public class TransportSubRouteController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransportSubRouteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<object> list = new();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM edu.TransportSubRouteMaster",
                con);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    subRouteCd = dr["subRouteCd"],
                    subRouteCode = dr["subRouteCode"],
                    subRouteName = dr["subRouteName"],
                    rCd = dr["rCd"],
                    descr = dr["Descr"],
                    userID = dr["UserID"]
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
                "SELECT * FROM edu.TransportSubRouteMaster WHERE subRouteCd=@subRouteCd",
                con);

            cmd.Parameters.AddWithValue("@subRouteCd", id);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
                return NotFound(new { Message = "Record Not Found" });

            return Ok(new
            {
                subRouteCd = dr["subRouteCd"],
                subRouteCode = dr["subRouteCode"],
                subRouteName = dr["subRouteName"],
                rCd = dr["rCd"],
                descr = dr["Descr"],
                userID = dr["UserID"]
            });
        }

        // INSERT
        [HttpPost]
        public IActionResult Insert([FromBody] TransportSubRouteMaster model)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "edu.sp_TransportSubRouteMaster_Insert",
                con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@subRouteCd", model.subRouteCd);
            cmd.Parameters.AddWithValue("@subRouteCode", model.subRouteCode);
            cmd.Parameters.AddWithValue("@subRouteName", model.subRouteName);
            cmd.Parameters.AddWithValue("@rCd", model.rCd);
            cmd.Parameters.AddWithValue("@Descr", model.Descr ?? "");
            cmd.Parameters.AddWithValue("@LoginName", model.LoginName);
            cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@UserID", model.UserID);

            DataTable dt = new();

            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return Ok(new
            {
                Message = dt.Rows[0]["Message"].ToString()
            });
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TransportSubRouteMaster model)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(@"
        UPDATE edu.TransportSubRouteMaster
        SET
            subRouteCode=@subRouteCode,
            subRouteName=@subRouteName,
            rCd=@rCd,
            Descr=@Descr,
            LoginName=@LoginName,
            lUserDt=@lUserDt,
            UserID=@UserID
        WHERE subRouteCd=@subRouteCd", con);

            cmd.Parameters.AddWithValue("@subRouteCd", id);
            cmd.Parameters.AddWithValue("@subRouteCode", model.subRouteCode);
            cmd.Parameters.AddWithValue("@subRouteName", model.subRouteName);
            cmd.Parameters.AddWithValue("@rCd", model.rCd);
            cmd.Parameters.AddWithValue("@Descr", model.Descr ?? "");
            cmd.Parameters.AddWithValue("@LoginName", model.LoginName);
            cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@UserID", model.UserID);

            con.Open();
            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound(new { Message = "Record Not Found" });

            return Ok(new
            {
                Message = "Sub Route updated successfully."
            });
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM edu.TransportSubRouteMaster WHERE subRouteCd=@subRouteCd",
                con);

            cmd.Parameters.AddWithValue("@subRouteCd", id);

            con.Open();

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound(new { Message = "Record Not Found" });

            return Ok(new
            {
                Message = "Sub Route deleted successfully."
            });
        }
    }



}
