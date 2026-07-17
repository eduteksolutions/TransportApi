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
       (@MobileNo,@OTP,'N',GETUTCDATE(),@UserId,@UserType)";

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
            if (string.IsNullOrWhiteSpace(request.MobileNo) || string.IsNullOrWhiteSpace(request.OTP))
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

                string otpQuery = @"SELECT COUNT(*)
                            FROM OTPhistroytbl
                            WHERE MobileNo=@MobileNo
                            AND OTP=@OTP
                            AND Status='N'
                            AND ctime_Stamp>=DATEADD(MINUTE,-10,GETUTCDATE())";
                using SqlCommand otpCmd = new SqlCommand(otpQuery, con);
                otpCmd.Parameters.AddWithValue("@MobileNo", request.MobileNo.Trim());
                otpCmd.Parameters.AddWithValue("@OTP", request.OTP.Trim());
                int count = Convert.ToInt32(await otpCmd.ExecuteScalarAsync());
                if (count == 0)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Invalid or expired OTP."
                    });
                }
                //Y Means Cusomed



                string updateQuery = @"UPDATE OTPhistroytbl
                               SET Status='Y'
                               WHERE MobileNo=@MobileNo
                               AND OTP=@OTP";
                using SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
                updateCmd.Parameters.AddWithValue("@OTP", request.OTP);
                await updateCmd.ExecuteNonQueryAsync();
                string userQuery = @"SELECT TOP 1
                             UserId,
                             
                             MobileNo
                             FROM OTPhistroytbl
                             WHERE MobileNo=@MobileNo";
                using SqlCommand userCmd = new SqlCommand(userQuery, con);
                userCmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
                int userId;
                string userName, mobileNo;
                await using (SqlDataReader reader = await userCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "User not found."
                        });
                    }
                    userId = Convert.ToInt32(reader["UserId"]);
                    
                    mobileNo = reader["MobileNo"].ToString();
                }
                // Generate JWT token now that we have the user's identity
                var tokenResult = _jwtService.GenerateToken(userId);
                return Ok(new
                {
                    success = true,
                    message = "Login successful.",
                    token = tokenResult.Token,
                    expiresOn = tokenResult.Expiry,
                    data = new
                    {
                        userId,
                       
                        mobileNo
                    }
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
    }
}