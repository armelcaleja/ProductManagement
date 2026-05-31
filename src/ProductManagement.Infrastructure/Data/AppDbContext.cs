using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Self-referencing hierarchy: ParentPackageId is nullable for root packages
            modelBuilder.Entity<Package>()
                .HasMany(p => p.PackageList)
                .WithOne()
                .HasForeignKey(p => p.ParentPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global query filters for soft delete
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Item>().HasQueryFilter(i => !i.IsDeleted);
            modelBuilder.Entity<Package>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<PackageType>().HasQueryFilter(pt => !pt.IsDeleted);
            modelBuilder.Entity<PackageItem>().HasQueryFilter(pi => !pi.IsDeleted);
        }
    }
}
