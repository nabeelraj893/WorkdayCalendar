using Microsoft.EntityFrameworkCore;
using WorkdayCalendar.Models;

namespace WorkdayCalendar
{
    public class AppDbContext : DbContext
    {
        // DbSets represent tables in the database
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<WorkHours> WorkHours { get; set; }

        // Constructor that accepts DbContextOptions for dependency injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
    }
}
