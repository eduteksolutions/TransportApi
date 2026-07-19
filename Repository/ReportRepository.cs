using Microsoft.Data.SqlClient;
using TransportApi.Data;
using TransportApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TransportApi.Repository
{
    public class ReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Student_Login_Details>> GetAllChildrenWithDeviceId()
        {
            return await _context.Student_Login_Details
                .Where(x => !string.IsNullOrEmpty(x.mbl_Device_ID))
                .ToListAsync();
        }
        public async Task<List<TodayBirthReport>> GetTodayBirthDayList()
        {
         

            return await _context.TodayBirthReports
                .FromSqlRaw("EXEC sp_TodayBirthDayListAllUser ")
                .ToListAsync();
        }
    }
}
