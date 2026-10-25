using ChatingApp.BackEnd.Entities;
using ChatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChatingApp.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<MemberLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER" },
                    new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN" },
                    new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR" }
                );

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(mr => mr.MessagesReceived)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(ms => ms.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

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

            // بيخلي كل التواريخ الـ nullable تتخزن وتتقرأ كـ UTC:
            // - وقت الحفظ: بيحول القيمة لـ UTC بـ ToUniversalTime()
            // - وقت القراءة: الداتابيز بترجع Kind = Unspecified، فبنحط عليها Utc بـ SpecifyKind
            //   (من غير ما نغير القيمة) عشان الـ JSON يطلع فيه Z والـ Angular ميزحزحش الوقت.
            // ملاحظة: بيشتغل على DateTime? بس، الـ DateTime العادي محتاج converter لوحده.
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }
    }
}
