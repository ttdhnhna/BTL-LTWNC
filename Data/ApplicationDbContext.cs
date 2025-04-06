using BTL_LTWNC.Models;
using Microsoft.EntityFrameworkCore;

namespace BTL_LTWNC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Vehicle> tbl_Vehicle { get; set; }

        public DbSet<Post> tbl_Post { get; set; }
    }
}
