using Microsoft.EntityFrameworkCore;
using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileEntity> Files { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Balance> Balances { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileEntity>()
                .Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(260);

            modelBuilder.Entity<Account>()
                .Property(a => a.AccountCode)
                .HasMaxLength(50);

            modelBuilder.Entity<Account>()
                .Property(a => a.AccountName)
                .IsRequired()
                .HasMaxLength(250);

        }
    }
}
