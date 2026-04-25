

using Microsoft.EntityFrameworkCore;
namespace TransportApi.Data

{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext>  options):base(options)
        { 
            
        }

    }
}
