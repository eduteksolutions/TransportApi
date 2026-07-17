using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;

namespace TransportApi.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    public class StudentInboxController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StudentInboxController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] StudentInbox model)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                con.Open();

                string query = @"
            INSERT INTO tblStudentInbox
            (
                AdmCd,
                MsgText,
                ImgURL,
                DownLink,
                Description,
                TDate,
                UserID,
                ClassCd,
                SecCd,
                URL_Date_Sheet,
                ToFaculty
            )
            VALUES
            (
                @AdmCd,
                @MsgText,
                @ImgURL,
                @DownLink,
                @Description,
                @TDate,
                @UserID,
                @ClassCd,
                @SecCd,
                @URL_Date_Sheet,
                @ToFaculty
            )";


                using SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@AdmCd", model.AdmCd);
                cmd.Parameters.AddWithValue("@MsgText", model.MsgText ?? "");
                cmd.Parameters.AddWithValue("@ImgURL",
                    (object?)model.ImgURL ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DownLink",
                    (object?)model.DownLink ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description",
                    (object?)model.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TDate",
                    model.TDate == DateTime.MinValue ? DateTime.Now : model.TDate);
                cmd.Parameters.AddWithValue("@UserID", model.UserID);
                cmd.Parameters.AddWithValue("@ClassCd",
                    (object?)model.ClassCd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SecCd",
                    (object?)model.SecCd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@URL_Date_Sheet",
                    (object?)model.URL_Date_Sheet ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ToFaculty", model.ToFaculty);


                int result = cmd.ExecuteNonQuery();


                if (result > 0)
                {
                    return Ok(new
                    {
                        status = true,
                        message = "Student inbox created successfully"
                    });
                }

                return BadRequest(new
                {
                    status = false,
                    message = "Insert failed"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
    }
}
