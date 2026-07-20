using ChatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .Property(u => u.ID)
                .HasDefaultValueSql("NEWID()");
        }

        public DbSet<AppUser> Users { get; set; }
    }
}
