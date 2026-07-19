using Microsoft.EntityFrameworkCore;
using TransportApi.Models;
using TransportApi.Models.TransportApi.Models;
namespace TransportApi.Data

{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>  options):base(options)
        { 
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodayBirthReport>().HasNoKey();
        }
        public DbSet<VehicleLiveLocation> VehicleLiveLocations { get; set; }
        public DbSet<VehicleDeviceLiveLocation> VehicleDeviceLiveLocations { get; set; }
        public DbSet<SchoolGpsSetting> SchoolGpsSettings { get; set; }
        public DbSet<StudentLocation> StudentLocations { get; set; }
        public DbSet<TransportBusProximityNotificationLogs> TransportBusProximityNotificationLogs { get; set; }
        public DbSet<Admission> Admission { get; set; }
        public DbSet<AdmissionDtl> AdmissionDTl { get; set; }
        public DbSet<TransportStationMaster> TransportStationMaster { get; set; }

        public DbSet<TodayBirthReport> TodayBirthReports { get; set; }
        public DbSet<Student_Login_Details> Student_Login_Details { get; set; }


    }
}
