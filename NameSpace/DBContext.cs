using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NameSpace.Models;


namespace NameSpace
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Användarnas tabell
        public DbSet<NameInfo> NameInfos { get; set; }
        public DbSet<UserReaction> UserReactions { get; set; }
        public DbSet<ConfirmPartner> ConfirmPartner { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NameInfo>()
                .HasIndex(s => new {s.Name, s.Gender})
                .IsUnique();

            //User reactions config
            modelBuilder.Entity<UserReaction>()
           .HasKey(tc => new { tc.UserId, tc.NameInfoId }); // Primärnyckel är sammansatt av UserId och PostId

            modelBuilder.Entity<UserReaction>()
                .HasOne(tc => tc.User)
                .WithMany(u => u.UserReactions)
                .HasForeignKey(tc => tc.UserId);


            base.OnModelCreating(modelBuilder);
        }
    } 
}
