using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TransportApi.Services; // Ensure this matches your service's namespace

namespace TransportApi.Controllers
{
    public class NotificationRequest
    {
        public string noticesubject { get; set; }
        public string noticetext { get; set; }
        public int HrdID { get; set; }
        public string sDate { get; set; }
        public int _userid { get; set; }
        public int _notificationid { get; set; }
        public int _cmdtype { get; set; }
    }

    public class NotificationRequestToken
    {
        public string DeviceToken { get; set; }
        public string FcmToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        // Inject both Configuration and your custom Notification Service here
        public NotificationController(IConfiguration configuration, INotificationService notificationService)
        {
            _configuration = configuration;
            _notificationService = notificationService;
        }

        [HttpGet("api/firebase/token")]
        public async Task<IActionResult> GetAccessToken()
        {
            try
            {
                string accessToken = await _notificationService.GetAccessTokenAsync();
                return Ok(new { token = accessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Code = "500", status = "False", Message = ex.Message });
            }
        }

        [HttpPost("api/send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequestToken request)
        {
            if (string.IsNullOrEmpty(request.DeviceToken))
                return BadRequest("Missing device token.");

            var result = await _notificationService.SendMessageAsync(request.DeviceToken, request.Title, request.Body);
            return Ok(result);
        }

        [HttpGet("api/send-notification")]
        public async Task<IActionResult> SendNotification(string deviceToken, string title, string body)
        {
            if (string.IsNullOrEmpty(deviceToken))
                return BadRequest("Missing device token.");

            var result = await _notificationService.SendMessageAsync(deviceToken, title, body);
            return Ok(new { result });
        }

        [HttpGet("api/get-notices")]
        public IActionResult GetNotices(int instituteCD, int HrdID, string token)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var records = new List<Dictionary<string, object>>();

            using (SqlCommand cmd = new SqlCommand("usp_GetNotices", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InstituteCD", instituteCD);
                cmd.Parameters.AddWithValue("@HrdID", HrdID);

                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            records.Add(row);
                        }
                    }

                    if (records.Count > 0) return Ok(records);

                    return Ok(new[] { new { Code = "301", status = "False", Message = "Records Not Found." } });
                }
                catch (Exception ex)
                {
                    return Ok(new[] { new { Code = "500", status = "False", Message = $"An error occurred: {ex.Message}" } });
                }
            }
        }

        [HttpGet("api/SendPushNotificationAdmCdBy")]
        public async Task<IActionResult> SendPushNotificationAdmCdBy(string noticesubject, string noticetext, string sDate, int userid, int admcd)
        {
            var deviceTokens = new List<string>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string selectQuery = "SELECT mbl_Device_ID FROM Student_Login_Details WHERE UserId = @UserId AND admcd = @AdmCd ORDER BY admCd";
                using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    cmd.Parameters.AddWithValue("@AdmCd", admcd);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string token = reader[0]?.ToString();
                            if (!string.IsNullOrEmpty(token) && token.Length > 1) deviceTokens.Add(token);
                        }
                    }
                }

                int k = 0;
                string insertQuery = @"INSERT INTO tblStudentInbox (UserID, AdmCd, MsgText, ImgURL, tDate, DownLink, Description) 
                                       VALUES (@UserID, @AdmCd, @MsgText, '', GETDATE(), '', @Description)";

                foreach (var token in deviceTokens)
                {
                    await _notificationService.SendMessageAsync(token, noticesubject, noticetext);

                    using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con))
                    {
                        cmdInsert.Parameters.AddWithValue("@UserID", userid);
                        cmdInsert.Parameters.AddWithValue("@AdmCd", admcd);
                        cmdInsert.Parameters.AddWithValue("@MsgText", noticesubject);
                        cmdInsert.Parameters.AddWithValue("@Description", noticetext);
                        cmdInsert.ExecuteNonQuery();
                    }
                    k = 1;
                }

                if (k > 0)
                    return Ok(new[] { new { Code = "200", status = "True", Message = "Notification Added" } });

                return Ok(new[] { new { Code = "201", status = "False", Message = "Notification Not Added" } });
            }
        }

        [HttpGet("api/SendPushNotificationByClass")]
        public async Task<IActionResult> SendPushNotificationByClass(int ClassCd, int SectionCd, int UserID, string Subject, string Text)
        {
            var deviceTokens = new List<string>();

            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            using (SqlCommand cmd = new SqlCommand("GetStudentDeviceId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClassCd", ClassCd);
                cmd.Parameters.AddWithValue("@UserID", UserID);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string token = reader[0]?.ToString();
                        if (!string.IsNullOrEmpty(token) && token.Length > 1) deviceTokens.Add(token);
                    }
                }
            }

            int k = 0;
            foreach (var token in deviceTokens)
            {
                await _notificationService.SendMessageAsync(token, Subject, Text);
                k = 1;
            }

            if (k > 0)
                return Ok(new[] { new { Code = "200", status = "True", Message = "Fetch Successfully" } });

            return Ok(new[] { new { Code = "201", status = "False", Message = "Fetch Not Successfully" } });
        }

        [HttpGet("api/SendPushNotificationByClassSection")]
        public async Task<IActionResult> SendPushNotificationByClassSection(int ClassCd, int SectionCd, int UserID, int HrdID, string Subject, string Text)
        {
            int newNoticeID = 0;
            int sentCount = 0;

            try
            {
                using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                using (SqlCommand cmd = new SqlCommand("edu.InsertNotice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@Subject", Subject);
                    cmd.Parameters.AddWithValue("@Text", Text);
                    cmd.Parameters.AddWithValue("@HrdID", HrdID);
                    cmd.Parameters.AddWithValue("@NoticeDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ClassCD", ClassCd);
                    cmd.Parameters.AddWithValue("@SecCd", SectionCd);
                    cmd.Parameters.AddWithValue("@FacultyCd", 1);

                    SqlParameter outParam = new SqlParameter("@NewNoticeID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    newNoticeID = Convert.ToInt32(outParam.Value);
                }

                var deviceTokens = new List<string>();
                using (SqlCommand cmdTokens = new SqlCommand("GetAdmissionWithDevice_ByClassSection", con))
                {
                    cmdTokens.CommandType = CommandType.StoredProcedure;
                    cmdTokens.Parameters.AddWithValue("@ClassCd", ClassCd);
                    cmdTokens.Parameters.AddWithValue("@SectionCd", SectionCd);
                    cmdTokens.Parameters.AddWithValue("@UserID", UserID);

                    using (SqlDataReader reader = cmdTokens.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string token = reader[3]?.ToString();
                            if (!string.IsNullOrEmpty(token)) deviceTokens.Add(token);
                        }
                    }
                }

                foreach (var token in deviceTokens)
                {
                    await _notificationService.SendMessageAsync(token, Subject, Text);
                    sentCount++;
                }

                return Ok(new[] { new { Code = "200", status = "True", Message = $"Notice saved (ID: {newNoticeID}) and notifications sent to {sentCount} student(s)." } });
            }
            catch (Exception ex)
            {
                return Ok(new[] { new { Code = "500", status = "False", Message = $"Error: {ex.Message}" } });
            }
        }

        [HttpGet("api/addNotification")]
        public IActionResult AddNotificationLegacy(string noticesubject, string noticetext, int HrdID, string sDate, int _userid, int _notificationid, int _cmdtype)
        {
            int k = 0;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                if (_cmdtype == 1)
                {
                    int cid = 1;
                    string maxQuery = "SELECT ISNULL(Max(NoticeID), 0) + 1 FROM tblNoticeMaster WHERE UserID = @UserID";
                    using (SqlCommand cmdMax = new SqlCommand(maxQuery, con))
                    {
                        cmdMax.Parameters.AddWithValue("@UserID", _userid);
                        cid = Convert.ToInt32(cmdMax.ExecuteScalar());
                    }

                    string insertQuery = "INSERT INTO tblNoticeMaster(NoticeID, NoticeSubject, NoticeText, hrdid, NoticeDate, UserID) VALUES (@NoticeID, @Subject, @Text, @HrdID, @Date, @UserID)";
                    using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con))
                    {
                        cmdInsert.Parameters.AddWithValue("@NoticeID", cid);
                        cmdInsert.Parameters.AddWithValue("@Subject", noticesubject);
                        cmdInsert.Parameters.AddWithValue("@Text", noticetext);
                        cmdInsert.Parameters.AddWithValue("@HrdID", HrdID);
                        cmdInsert.Parameters.AddWithValue("@Date", sDate);
                        cmdInsert.Parameters.AddWithValue("@UserID", _userid);
                        k = cmdInsert.ExecuteNonQuery();
                    }
                }
                else
                {
                    string deleteQuery = "DELETE FROM tblNoticeMaster WHERE UserID = @UserID AND NoticeID = @NoticeID";
                    using (SqlCommand cmdDel = new SqlCommand(deleteQuery, con))
                    {
                        cmdDel.Parameters.AddWithValue("@UserID", _userid);
                        cmdDel.Parameters.AddWithValue("@NoticeID", _notificationid);
                        cmdDel.ExecuteNonQuery();
                    }

                    string insertQuery = "INSERT INTO tblNoticeMaster(NoticeID, NoticeSubject, NoticeText, hrdid, NoticeDate, UserID) VALUES (@NoticeID, @Subject, @Text, @HrdID, @Date, @UserID)";
                    using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con))
                    {
                        cmdInsert.Parameters.AddWithValue("@NoticeID", _notificationid);
                        cmdInsert.Parameters.AddWithValue("@Subject", noticesubject);
                        cmdInsert.Parameters.AddWithValue("@Text", noticetext);
                        cmdInsert.Parameters.AddWithValue("@HrdID", HrdID);
                        cmdInsert.Parameters.AddWithValue("@Date", sDate);
                        cmdInsert.Parameters.AddWithValue("@UserID", _userid);
                        k = cmdInsert.ExecuteNonQuery();
                    }
                }

                if (k > 0)
                    return Ok(new[] { new { Code = "200", status = "True", Message = "Notification Added" } });

                return Ok(new[] { new { Code = "201", status = "False", Message = "Notification Not Added" } });
            }
        }

        [HttpPost("api/add-Notification")]
        public async Task<IActionResult> AddNotification([FromBody] NotificationRequest model)
        {
            int k = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("edu.sp_notification", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NoticeSubject", model.noticesubject);
                        cmd.Parameters.AddWithValue("@NoticeText", model.noticetext);
                        cmd.Parameters.AddWithValue("@hrdid", model.HrdID);
                        cmd.Parameters.AddWithValue("@UserID", model._userid);
                        cmd.Parameters.AddWithValue("@StatementType", model._cmdtype);

                        con.Open();
                        k = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (model._cmdtype == 1 && k == 1)
                    {
                        var deviceTokens = new List<string>();
                        string tokenQuery = "SELECT mbl_Device_ID FROM Student_Login_Details WHERE UserID = @UserID ORDER BY admCd";
                        using (SqlCommand cmdTokens = new SqlCommand(tokenQuery, con))
                        {
                            cmdTokens.Parameters.AddWithValue("@UserID", model._userid);
                            using (SqlDataReader reader = cmdTokens.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string token = reader["mbl_Device_ID"]?.ToString();
                                    if (!string.IsNullOrWhiteSpace(token)) deviceTokens.Add(token);
                                }
                            }
                        }

                        foreach (var token in deviceTokens)
                        {
                            await _notificationService.SendMessageAsync(token, model.noticesubject, model.noticetext);
                        }
                    }
                }

                if (k == 1) return Ok(new[] { new { Code = "200", status = "True", Message = "Notification Added" } });
                if (k == 2) return Ok(new[] { new { Code = "200", status = "True", Message = "Notifications Retrieved" } });

                return Ok(new[] { new { Code = "202", status = "False", Message = "Operation Failed" } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("api/deletenotification")]
        public IActionResult DeleteNotification(int _notificationid, int _userid, int deleteid)
        {
            string query = "DELETE FROM tblNoticeMaster WHERE UserID = @UserID AND NoticeID = @NoticeID";
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserID", _userid);
                cmd.Parameters.AddWithValue("@NoticeID", _notificationid);

                con.Open();
                int k = cmd.ExecuteNonQuery();

                if (k > 0) return Ok(new[] { new { Code = "200", status = "True", Message = "Deleted" } });
                return Ok(new[] { new { Code = "201", status = "False", Message = "Not Deleted" } });
            }
        }
    }
}