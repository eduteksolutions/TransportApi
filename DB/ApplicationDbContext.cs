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
        public DbSet<VehicleLiveLocation> VehicleLiveLocations { get; set; }
        public DbSet<VehicleDeviceLiveLocation> VehicleDeviceLiveLocations { get; set; }
    }
}
