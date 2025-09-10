using Microsoft.EntityFrameworkCore;
using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileEntity> Files { get; set; } = null!;
        public DbSet<AccountClass> AccountClasses { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Balance> Balances { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileEntity>()
                .Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(260);


            modelBuilder.Entity<AccountClass>()
                .Property(ac => ac.ClassName)
                .IsRequired()
                .HasMaxLength(260);

            modelBuilder.Entity<AccountClass>()
                .Property(ac => ac.ClassCode)
                .HasMaxLength(20);
        }
    }
}
