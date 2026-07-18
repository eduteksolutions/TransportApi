using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;
using TransportApi.Services;

namespace TransportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SmsService _smsService;
        private readonly JwtTokenService _jwtService;

        public LoginController(IConfiguration configuration,
                               SmsService smsService,
                               JwtTokenService jwtService)
        {
            _configuration = configuration;
            _smsService = smsService;
            _jwtService = jwtService;
        }

        [HttpPost("SendOTP")]
        /*  public async Task<IActionResult> SendOTP([FromBody] SendOtpRequest request)
          {
              if (string.IsNullOrWhiteSpace(request.MobileNo))
              {
                  return BadRequest(new
                  {
                      success = false,
                      message = "Mobile number is required."
                  });
              }

              string otp = new Random().Next(100000, 999999).ToString();

              try
              {
                  using SqlConnection con = new SqlConnection(
                      _configuration.GetConnectionString("DefaultConnection"));

                  await con.OpenAsync();

                  string insertQuery = @"INSERT INTO OTPhistroytbl
                 (MobileNo,OTP,Status,ctime_Stamp,userID,UserType)
                 VALUES
                 (@MobileNo,@OTP,'Y',GETUTCDATE(),@UserId,@UserType)";

                  using SqlCommand cmd = new SqlCommand(insertQuery, con);
                  cmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
                  cmd.Parameters.AddWithValue("@OTP", otp);
                  cmd.Parameters.AddWithValue("@UserId", request.UserID);
                  cmd.Parameters.AddWithValue("@UserType", request.UserType);


                  await cmd.ExecuteNonQueryAsync();

                  string otpForSms = "ZioSchool:-" + otp;

                  string smsText =
                      $"Dear User, Your OTP for login on the Edutek Portal is {otpForSms}. " +
                      $"This OTP is valid for 5 minutes. Please do not share it with anyone. " +
                      $"Website: https://eduteksolutions.in Regards, Team Edutek";

                  string smsUrl =
                      "https://sms.omnitechintegrators.com/fe/api/v1/multiSend" +
                      $"?username={_configuration["SmsSettings:Username"]}" +
                      $"&password={_configuration["SmsSettings:Password"]}" +
                      "&unicode=false" +
                      "&from=EDUTKS" +
                      $"&to={request.MobileNo}" +
                      "&dltContentId=1707178309927469346" +
                      "&dltPrincipalEntityId=1201159350821274881" +
                      $"&text={WebUtility.UrlEncode(smsText)}";

                  using HttpClient client = new HttpClient();
                  HttpResponseMessage smsResponse = await client.GetAsync(smsUrl);
                  string smsResult = await smsResponse.Content.ReadAsStringAsync();
                  Console.WriteLine($"[SendOTP] SMS gateway response: {smsResult}");

                  return Ok(new
                  {
                      success = smsResponse.IsSuccessStatusCode,
                      message = "OTP sent successfully."
                  });
              }
              catch (Exception ex)
              {
                  return StatusCode(500, new
                  {
                      success = false,
                      message = "Failed to send OTP.",
                      error = ex.Message
                  });
              }
          }*/
        
        public async Task<IActionResult> SendOTP([FromBody] SendOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNo))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Mobile number is required."
                });
            }

            string otp = new Random().Next(1000, 9999).ToString();

            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                await con.OpenAsync();

                string insertQuery = @"INSERT INTO OTPhistroytbl
       (MobileNo,OTP,Status,ctime_Stamp,userID,UserType)
       VALUES
       (@MobileNo,@OTP,'N',GetDate(),@UserId,@UserType)";

                using SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
                cmd.Parameters.AddWithValue("@OTP", otp);
                cmd.Parameters.AddWithValue("@UserId", request.UserID);
                cmd.Parameters.AddWithValue("@UserType", request.UserType);

                await cmd.ExecuteNonQueryAsync();

                string otpForSms = "ZioSchool:-" + otp;
                string smsText =
                    $"Dear User, Your OTP for login on the Edutek Portal is {otpForSms}. " +
                    $"This OTP is valid for 5 minutes. Please do not share it with anyone. " +
                    $"Website: https://eduteksolutions.in Regards, Team Edutek";

                /* ---------- SMS API CALL DISABLED FOR TESTING ----------
                string smsUrl =
                    "https://sms.omnitechintegrators.com/fe/api/v1/multiSend" +
                    $"?username={_configuration["SmsSettings:Username"]}" +
                    $"&password={_configuration["SmsSettings:Password"]}" +
                    "&unicode=false" +
                    "&from=EDUTKS" +
                    $"&to={request.MobileNo}" +
                    "&dltContentId=1707178309927469346" +
                    "&dltPrincipalEntityId=1201159350821274881" +
                    $"&text={WebUtility.UrlEncode(smsText)}";

                using HttpClient client = new HttpClient();
                HttpResponseMessage smsResponse = await client.GetAsync(smsUrl);
                string smsResult = await smsResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"[SendOTP] SMS gateway response: {smsResult}");
                ---------------------------------------------------------- */

                Console.WriteLine($"[SendOTP] DUMMY MODE — OTP for {request.MobileNo} is {otp} (SMS not sent)");

                return Ok(new
                {
                    success = true,
                    message = "OTP generated (dummy mode — SMS not sent).",
                    otp = otp   // returned directly for testing only — remove before going live
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send OTP.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("LoginWithOTP")]

      
        public async Task<IActionResult> LoginWithOTP([FromBody] LoginOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Device Id is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.MobileNo) ||
                string.IsNullOrWhiteSpace(request.OTP))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Mobile number and OTP are required."
                });
            }


            try
            {
                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                await con.OpenAsync();


                int userId = 0;


                // ---------------- VERIFY OTP ----------------

                string otpQuery = @"
        SELECT TOP 1 UserID
        FROM OTPhistroytbl
        WHERE LTRIM(RTRIM(MobileNo))=@MobileNo
        AND LTRIM(RTRIM(OTP))=@OTP
        AND LTRIM(RTRIM(Status))='N'
        AND ctime_Stamp >= DATEADD(MINUTE,-10,GETDATE())
        ORDER BY ID DESC";


                using (SqlCommand cmd = new SqlCommand(otpQuery, con))
                {
                    cmd.Parameters.AddWithValue("@MobileNo", request.MobileNo.Trim());
                    cmd.Parameters.AddWithValue("@OTP", request.OTP.Trim());


                    object result = await cmd.ExecuteScalarAsync();


                    if (result == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Invalid or expired OTP."
                        });
                    }


                    userId = Convert.ToInt32(result);
                }



                // ---------------- UPDATE OTP STATUS ----------------

                string updateOtp = @"
        UPDATE OTPhistroytbl
        SET Status='Y'
        WHERE MobileNo=@MobileNo
        AND OTP=@OTP
        AND UserID=@UserID";


                using (SqlCommand cmd = new SqlCommand(updateOtp, con))
                {
                    cmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
                    cmd.Parameters.AddWithValue("@OTP", request.OTP);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    await cmd.ExecuteNonQueryAsync();
                }




                // ---------------- CHECK FACULTY ----------------

                int facultyCode = 0;


                string facultyQuery = @"
        SELECT TOP 1
            code AS Faculty_Cd
        FROM HRDStaffMaster
        WHERE Phone=@MobileNo
        AND UserID=@UserId";


                using (SqlCommand cmd = new SqlCommand(facultyQuery, con))
                {
                    cmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
                    cmd.Parameters.AddWithValue("@UserId", userId);


                    object result = await cmd.ExecuteScalarAsync();


                    if (result == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Faculty not found."
                        });
                    }


                    facultyCode = Convert.ToInt32(result);
                }




                // ---------------- UPDATE DEVICE ----------------

                string updateDevice = @"
        UPDATE HRDStaffMaster
        SET mbl_Device_ID=@DeviceId,
            mbl_Device_Type=@DeviceType
        WHERE code=@FacultyCd
        AND UserID=@UserId";


                using (SqlCommand cmd = new SqlCommand(updateDevice, con))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", request.DeviceId);
                    cmd.Parameters.AddWithValue("@DeviceType", request.DeviceType ?? "");
                    cmd.Parameters.AddWithValue("@FacultyCd", facultyCode);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    await cmd.ExecuteNonQueryAsync();
                }




                // ---------------- GET FACULTY DATA ----------------

                object facultyData = null;


                string profileQuery = @"
        SELECT
            code AS Faculty_Cd,
            sName AS Faculty_Name,
            Phone AS Mobile,
            pic AS Pic,
            Qualification,
            Address,
            UserID
        FROM HRDStaffMaster
        WHERE code=@FacultyCd
        AND UserID=@UserID";



                using (SqlCommand cmd = new SqlCommand(profileQuery, con))
                {
                    cmd.Parameters.AddWithValue("@FacultyCd", facultyCode);
                    cmd.Parameters.AddWithValue("@UserID", userId);


                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();


                    if (!await reader.ReadAsync())
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Faculty details not found."
                        });
                    }


                    facultyData = new
                    {
                        code = 200,
                        status = true,
                        message = "Logged Successfully",

                        UserID = Convert.ToInt32(reader["UserID"]),
                        Faculty_Cd = Convert.ToInt32(reader["Faculty_Cd"]),
                        Faculty_Name = reader["Faculty_Name"]?.ToString(),
                        Mobile = reader["Mobile"]?.ToString(),
                        Pic = reader["Pic"]?.ToString(),
                        Qualification = reader["Qualification"]?.ToString(),
                        Address = reader["Address"]?.ToString()
                    };
                }





                // ---------------- JWT TOKEN ----------------

                var tokenResult = _jwtService.GenerateToken(userId);



                return Ok(new
                {
                    success = true,
                    message = "Login successful.",

                    token = tokenResult.Token,
                    expiresOn = tokenResult.Expiry,

                    data = facultyData
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Login failed.",
                    error = ex.Message
                });
            }
        }
        private async Task<object> mGetLogin(int facultyCd, int userId)
        {
            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await con.OpenAsync();

            string query = @"
        SELECT
            empcode AS Faculty_Cd,
            fName AS Faculty_Name,
            MobileNo AS Mobile,
            imagePath AS Pic,
            Qualification,
            Address,
            UserID
        FROM TeacherProfile
        WHERE empcode = @FacultyCd
          AND UserID = @UserId";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@FacultyCd", facultyCd);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new
            {
                code = 200,
                status = true,
                message = "Logged Successfully",
                Faculty_Cd = reader["Faculty_Cd"],
                Faculty_Name = reader["Faculty_Name"],
                Mobile = reader["Mobile"],
                Pic = reader["Pic"],
                Qualification = reader["Qualification"],
                Address = reader["Address"],
                UserID = reader["UserID"]
            };
        }
    }
}