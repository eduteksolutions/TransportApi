using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportVehicleMasterController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransportVehicleMasterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/TransportVehicle/GetAll?userid=1
        [HttpGet("GetAll")]
        public IActionResult GetAll(int userid)
        {
            List<object> list = new();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                @"SELECT *
                  FROM TransportVehicleMaster
                  WHERE UserID = @UserID",
                con);

            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    vCd = dr["vCd"],
                    vTypeCode = dr["vTypeCode"],
                    vNo = dr["vNo"],
                    Capacity = dr["Capacity"],
                    purchDt = dr["purchDt"],
                    purchCost = dr["purchCost"],
                    BillNo = dr["BillNo"],
                    BillDate = dr["BillDate"],
                    Model = dr["Model"],
                    Milage = dr["Milage"],
                    ChasisNo = dr["ChasisNo"],
                    purchFrom = dr["purchFrom"],
                    Add2 = dr["Add2"],
                    Add3 = dr["Add3"],
                    vDescr = dr["vDescr"],
                    picPath = dr["picPath"]
                });
            }

            if (list.Count == 0)
            {
                return NotFound(new
                {
                    Message = "No vehicles found for this user."
                });
            }

            return Ok(list);
        }

        // GET: api/TransportVehicle/1/101
        [HttpGet("{id}/{userid}")]
        public IActionResult GetById(int id, int userid)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                @"SELECT *
                  FROM TransportVehicleMaster
                  WHERE vCd = @vCd
                    AND UserID = @UserID",
                con);

            cmd.Parameters.AddWithValue("@vCd", id);
            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
                return NotFound(new
                {
                    Message = "Vehicle not found."
                });

            return Ok(new
            {
                vCd = dr["vCd"],
                vTypeCode = dr["vTypeCode"],
                vNo = dr["vNo"],
                Capacity = dr["Capacity"],
                purchDt = dr["purchDt"],
                purchCost = dr["purchCost"],
                BillNo = dr["BillNo"],
                BillDate = dr["BillDate"],
                Model = dr["Model"],
                Milage = dr["Milage"],
                ChasisNo = dr["ChasisNo"],
                purchFrom = dr["purchFrom"],
                Add2 = dr["Add2"],
                Add3 = dr["Add3"],
                vDescr = dr["vDescr"],
                picPath = dr["picPath"],
                LoginName = dr["LoginName"],
                UserID = dr["UserID"]
            });
        }

        // POST: api/TransportVehicle
        [HttpPost]
        public IActionResult Insert([FromBody] TransportVehicleMaster model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand(
                    "edu.InsertTransportVehicleMaster",
                    con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@vTypeCode", model.vTypeCode);
                cmd.Parameters.AddWithValue("@vNo", model.vNo);
                cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
                cmd.Parameters.AddWithValue("@purchDt", model.purchDt);
                cmd.Parameters.AddWithValue("@purchCost", model.purchCost);
                cmd.Parameters.AddWithValue("@BillNo", model.BillNo);
                cmd.Parameters.AddWithValue("@BillDate", model.BillDate);
                cmd.Parameters.AddWithValue("@Model", model.Model);
                cmd.Parameters.AddWithValue("@Milage", model.Milage);
                cmd.Parameters.AddWithValue("@ChasisNo", model.ChasisNo);
                cmd.Parameters.AddWithValue("@purchFrom", model.purchFrom);
                cmd.Parameters.AddWithValue("@Add2", model.Add2);
                cmd.Parameters.AddWithValue("@Add3", model.Add3);
                cmd.Parameters.AddWithValue("@vDescr", model.vDescr);
                cmd.Parameters.AddWithValue("@picPath", model.picPath);
                cmd.Parameters.AddWithValue("@LoginName", model.LoginName);
                cmd.Parameters.AddWithValue("@UserID", model.UserID);

                SqlParameter transStatus =
                    new SqlParameter("@TransStatus", SqlDbType.Int);

                transStatus.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(transStatus);

                con.Open();
                cmd.ExecuteNonQuery();

                int result = Convert.ToInt32(transStatus.Value);

                if (result == -1)
                {
                    return BadRequest(new
                    {
                        Message = "Vehicle Number or Chassis Number already exists."
                    });
                }

                return Ok(new
                {
                    Message = "Vehicle inserted successfully."
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

        // PUT: api/TransportVehicle/1/101
        [HttpPut("{id}/{userid}")]
        public IActionResult Update(
            int id,
            int userid,
            [FromBody] TransportVehicleMaster model)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(@"
                UPDATE TransportVehicleMaster
                SET
                    vTypeCode = @vTypeCode,
                    vNo = @vNo,
                    Capacity = @Capacity,
                    purchDt = @purchDt,
                    purchCost = @purchCost,
                    BillNo = @BillNo,
                    BillDate = @BillDate,
                    Model = @Model,
                    Milage = @Milage,
                    ChasisNo = @ChasisNo,
                    purchFrom = @purchFrom,
                    Add2 = @Add2,
                    Add3 = @Add3,
                    vDescr = @vDescr,
                    picPath = @picPath,
                    LoginName = @LoginName,
                    lUserDt = @lUserDt
                WHERE vCd = @vCd
                  AND UserID = @UserID",
                con);

            cmd.Parameters.AddWithValue("@vCd", id);
            cmd.Parameters.AddWithValue("@UserID", userid);

            cmd.Parameters.AddWithValue("@vTypeCode", model.vTypeCode);
            cmd.Parameters.AddWithValue("@vNo", model.vNo);
            cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
            cmd.Parameters.AddWithValue("@purchDt", model.purchDt);
            cmd.Parameters.AddWithValue("@purchCost", model.purchCost);
            cmd.Parameters.AddWithValue("@BillNo", model.BillNo);
            cmd.Parameters.AddWithValue("@BillDate", model.BillDate);
            cmd.Parameters.AddWithValue("@Model", model.Model);
            cmd.Parameters.AddWithValue("@Milage", model.Milage);
            cmd.Parameters.AddWithValue("@ChasisNo", model.ChasisNo);
            cmd.Parameters.AddWithValue("@purchFrom", model.purchFrom);
            cmd.Parameters.AddWithValue("@Add2", model.Add2);
            cmd.Parameters.AddWithValue("@Add3", model.Add3);
            cmd.Parameters.AddWithValue("@vDescr", model.vDescr);
            cmd.Parameters.AddWithValue("@picPath", model.picPath);
            cmd.Parameters.AddWithValue("@LoginName", model.LoginName);
            cmd.Parameters.AddWithValue("@lUserDt", DateTime.Now.Date);

            con.Open();

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                return NotFound(new
                {
                    Message = "Vehicle not found."
                });
            }

            return Ok(new
            {
                Message = "Vehicle updated successfully.",
                UpdatedBy = userid
            });
        }

        // DELETE: api/TransportVehicle/1/101
        [HttpDelete("{id}/{userid}")]
        public IActionResult Delete(int id, int userid)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand(
                @"DELETE FROM TransportVehicleMaster
                  WHERE vCd = @vCd
                    AND UserID = @UserID",
                con);

            cmd.Parameters.AddWithValue("@vCd", id);
            cmd.Parameters.AddWithValue("@UserID", userid);

            con.Open();

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                return NotFound(new
                {
                    Message = "Vehicle not found."
                });
            }

            return Ok(new
            {
                Message = "Vehicle deleted successfully.",
                DeletedBy = userid
            });
        }
    }
}