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

        public DbSet<VehiclePostViewModel> tbl_Vehicle { get; set; }

        public DbSet<PostViewModel> tbl_Post { get; set; }
    }
}
