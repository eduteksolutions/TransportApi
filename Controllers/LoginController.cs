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

        public LoginController(IConfiguration configuration,
                               SmsService smsService)
        {
            _configuration = configuration;
            _smsService = smsService;
        }

        [HttpPost("SendOTP")]
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

            string otp = new Random().Next(100000, 999999).ToString();

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await con.OpenAsync();

            string insertQuery = @"INSERT INTO OTPhistroytbl
                                  (MobileNo,OTP,Status,ctime_Stamp)
                                  VALUES
                                  (@MobileNo,@OTP,'Y',GETDATE())";

            SqlCommand cmd = new SqlCommand(insertQuery, con);

            cmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
            cmd.Parameters.AddWithValue("@OTP", otp);

            await cmd.ExecuteNonQueryAsync();

            await _smsService.SendSmsAsync(
                 request.MobileNo,
                $"Your OTP is {otp}. It is valid for 5 minutes."
            );

           
            return Ok(new
            {
                success = true,
                message = "OTP sent successfully."
            });
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

            using SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await con.OpenAsync();

            string otpQuery = @"SELECT COUNT(*)
                                FROM OTPhistroytbl
                                WHERE MobileNo=@MobileNo
                                AND OTP=@OTP
                                AND Status='Y'
                                AND CreatedDate>=DATEADD(MINUTE,-5,GETDATE())";

            SqlCommand otpCmd = new SqlCommand(otpQuery, con);

            otpCmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
            otpCmd.Parameters.AddWithValue("@OTP", request.OTP);

            int count = Convert.ToInt32(await otpCmd.ExecuteScalarAsync());

            if (count == 0)
            {
                return Ok(new
                {
                    success = false,
                    message = "Invalid or expired OTP."
                });
            }

            string updateQuery = @"UPDATE OTPhistroytbl
                                   SET Status='N'
                                   WHERE MobileNo=@MobileNo
                                   AND OTP=@OTP";

            SqlCommand updateCmd = new SqlCommand(updateQuery, con);

            updateCmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);
            updateCmd.Parameters.AddWithValue("@OTP", request.OTP);

            await updateCmd.ExecuteNonQueryAsync();

            string userQuery = @"SELECT TOP 1
                                 UserId,
                                 UserName,
                                 MobileNo
                                 FROM Users
                                 WHERE MobileNo=@MobileNo";

            SqlCommand userCmd = new SqlCommand(userQuery, con);

            userCmd.Parameters.AddWithValue("@MobileNo", request.MobileNo);

            SqlDataReader reader = await userCmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Ok(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Login successful.",
                data = new
                {
                    userId = reader["UserId"],
                    userName = reader["UserName"],
                    mobileNo = reader["MobileNo"]
                }
            });
        }
    }
}