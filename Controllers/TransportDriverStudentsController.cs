using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
        public class TransportDriverStudentsController : ControllerBase
        {
            private readonly IConfiguration _configuration;

            public TransportDriverStudentsController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [HttpGet("routestudents")]
            public IActionResult GetRouteWiseStudents(int driverCode, int userId)
            {
                var response = new RouteModel();
                var students = new List<TransportStudent>();

                string connStr = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("edu.sp_GetTransDriverRoutesStudentList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DriverCode", driverCode);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        bool headerFilled = false;

                        while (dr.Read())
                        {
                            if (!headerFilled)
                            {
                                response.RouteCode = Convert.ToInt32(dr["RouteCode"]);
                                response.RouteName = dr["subRouteName"]?.ToString();
                                response.VehicleNo = dr["VehicleNo"]?.ToString();
                                response.MorningTime = dr["MorningTime"]?.ToString();
                                response.EveningTime = dr["EveningTime"]?.ToString();
                                response.ValidDate = dr["ValidDate"]?.ToString();

                                headerFilled = true;
                            }

                            if (dr["AdmCd"] != DBNull.Value)
                            {
                                students.Add(new TransportStudent
                                {
                                    AdmCd = Convert.ToInt32(dr["AdmCd"]),
                                    Name = dr["firstName"]?.ToString(),
                                    Class = dr["aClass"]?.ToString(),
                                    Section = dr["cSection"]?.ToString(),
                                    FatherName = dr["fName"]?.ToString(),
                                    MotherName = dr["mName"]?.ToString(),
                                    Address = dr["Address"]?.ToString()
                                });
                            }
                        }
                    }
                }

                response.Students = students;

                return Ok(response);
            }
        }
    }

