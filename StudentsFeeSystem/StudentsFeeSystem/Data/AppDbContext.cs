using Microsoft.EntityFrameworkCore;
using StudentsFeeSystem.Models;

namespace StudentsFeeSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.Student> Students { get; set; }
        public DbSet<FeeItem> FeeItems { get; set; }
        public DbSet<GenderFeeAssignment> GenderFeeAssignments { get; set; }

    }
}
