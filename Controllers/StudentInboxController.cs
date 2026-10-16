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
        
        private int GetIntSafe(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private string GetStringSafe(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetValue(ordinal)?.ToString() ?? string.Empty;
        }

        private DateTime GetDateTimeSafe(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
        }
        [HttpGet]
        public IActionResult GetInbox([FromQuery] string instituteID, [FromQuery] string admcd)
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                con.Open();

                string query = @"
            SELECT * FROM tblStudentInbox 
            WHERE UserID = @UserID AND AdmCd = @AdmCd
            ORDER BY tDate DESC";

                using SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserID", instituteID);
                cmd.Parameters.AddWithValue("@AdmCd", admcd);

                using SqlDataReader reader = cmd.ExecuteReader();
                List<StudentInbox> list = new List<StudentInbox>();

                while (reader.Read())
                {
                    list.Add(new StudentInbox
                    {
                        AdmCd = GetIntSafe(reader, "AdmCd"),
                        MsgText = GetStringSafe(reader, "MsgText"),
                        ImgURL = GetStringSafe(reader, "ImgURL"),
                        DownLink = GetStringSafe(reader, "DownLink"),
                        Description = GetStringSafe(reader, "Description"),
                        TDate = GetDateTimeSafe(reader, "tDate"), // Matches lowercase tDate from your DB
                        UserID = GetIntSafe(reader, "UserID"),
                        ClassCd = GetIntSafe(reader, "ClassCd"),
                        SecCd = GetIntSafe(reader, "SecCd"),
                        URL_Date_Sheet = GetStringSafe(reader, "URL_Date_Sheet"),
                        ToFaculty = GetIntSafe(reader, "toFaculty"), // Matches lowercase toFaculty from your DB

                        // If you added the column in SQL, use: GetStringSafe(reader, "IsRead")
                        // Otherwise, default it to "N" so Flutter can read it:
                        IsRead = "N"
                    });
                }

                return Ok(list);
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
