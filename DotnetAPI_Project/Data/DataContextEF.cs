using DotnetAPI_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI_Project.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _configuration;

        public DataContextEF(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }
        public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")!,
                                            sqlOptionsBuilder => sqlOptionsBuilder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(e => e.UserId);

            modelBuilder.Entity<UserSalary>()
                .ToTable("UserSalary")
                .HasKey(e => e.UserId);

            modelBuilder.Entity<UserJobInfo>()
                .ToTable("UserJobInfo")
                .HasKey(e => e.UserId);
        }
    }
}

