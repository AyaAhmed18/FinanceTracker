using FinanceTracker.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            b.Entity<Category>()
                .Property(c => c.Name).HasMaxLength(100);

            b.Entity<Transaction>()
                .Property(t => t.Amount).HasPrecision(18, 2);

            // نمنع multiple cascade paths في SQL Server
            b.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
