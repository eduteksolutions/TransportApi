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

        public async Task<List<TodayBirthReport>> GetTodayBirthDayList(int userId)
        {
         

            return await _context.TodayBirthReports
                .FromSqlRaw("EXEC sp_TodayBirthDayListAllUser ")
                .ToListAsync();
        }
    }
}
