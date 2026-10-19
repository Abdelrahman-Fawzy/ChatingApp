using ChatingApp.BackEnd.Entities;
using ChatingApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChatingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<MemberLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MemberLike>()
                .HasKey(ml => new { ml.SourceMemberID, ml.TargetMemberID });

            modelBuilder.Entity<MemberLike>()
                .HasOne(ml => ml.SourceMember)
                .WithMany(m => m.LikedMembers)
                .HasForeignKey(ml => ml.SourceMemberID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberLike>()
                .HasOne(ml => ml.TargetMember)
                .WithMany(m => m.LikedByMembers)
                .HasForeignKey(ml => ml.TargetMemberID)
                .OnDelete(DeleteBehavior.NoAction);

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                }
            }
        }
    }
}
