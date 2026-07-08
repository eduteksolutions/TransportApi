using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;



namespace TransportApi.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class DriverLoginController : ControllerBase
        {
            private readonly IConfiguration _configuration;

            public DriverLoginController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [HttpGet("DriverLogin")]
            public IActionResult DriverLogin(
                int instituteCode,
                int facultyCd,
                string password,
                string mbl_DeviceId,
                string mbl_DeviceType)
            {
                try
                {
                    if (string.IsNullOrEmpty(mbl_DeviceId))
                    {
                        return Ok(new[]
                        {
                        new
                        {
                            ResponseCode = "1"
                        }
                    });
                    }

                    using SqlConnection con = new SqlConnection(
                        _configuration.GetConnectionString("DefaultConnection"));

                    con.Open();

                    SqlCommand cmd = new SqlCommand("sp_DriverLogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FacultyCd", facultyCd);
                    cmd.Parameters.AddWithValue("@UserID", instituteCode);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@DeviceID", mbl_DeviceId);
                    cmd.Parameters.AddWithValue("@DeviceType", mbl_DeviceType);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        return Ok(new
                        {
                            code = "200",
                            status = true,
                            message = "Login Successful",

                            Faculty_Cd = dr["Faculty_Cd"],
                            Faculty_Name = dr["Faculty_Name"],
                            Mobile = dr["Mobile"],
                            pic = dr["pic"],
                            DOB = dr["DOB"],
                            Qualification = dr["Qualification"],
                            Address = dr["Address"],
                            ClassIncharge = dr["ClassIncharge"],
                            UserID = dr["UserID"],
                            InChargeClassCd = dr["InChargeClassCd"],
                            InChargeSecCd = dr["InChargeSecCd"],
                            instituteName = dr["instituteName"],
                            institueLogo = dr["institueLogo"],
                            InstituteAddress = dr["InstituteAddress"],
                            InstitutePhone = dr["InstitutePhone"],
                            InstituteState = dr["InstituteState"],
                            InstituteCountry = dr["InstituteCountry"],
                            InstituteAbout = dr["InstituteAbout"],
                            WebSiteLinks = dr["WebSiteLinks"],
                            Logo = dr["Logo"]
                        });
                    }

                    return Ok(new
                    {
                        code = "401",
                        status = false,
                        message = "Invalid Faculty Code or User ID",

                        Faculty_Cd = "",
                        Faculty_Name = "",
                        Mobile = "",
                        pic = "",
                        DOB = "",
                        Qualification = "",
                        Address = "",
                        ClassIncharge = "",
                        UserID = "",
                        InChargeClassCd = "",
                        InChargeSecCd = "",
                        instituteName = "",
                        institueLogo = "",
                        InstituteAddress = "",
                        InstitutePhone = "",
                        InstituteState = "",
                        InstituteCountry = "",
                        InstituteAbout = "",
                        WebSiteLinks = "",
                        Logo = ""
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        code = "500",
                        status = false,
                        message = ex.Message
                    });
                }
            }
        }
    }

