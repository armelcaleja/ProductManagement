using Microsoft.EntityFrameworkCore;
using ProductManagement.Model;
using System.Reflection.Emit;

namespace ProductManagement.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=DESKTOP-QEG9QV3;Database=ProductManagementDb;Trusted_Connection=True;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fix the self-referencing cascade path error
            modelBuilder.Entity<Package>()
                .HasMany(p => p.PackageList)
                .WithOne()
                .HasForeignKey(p => p.ParentPackageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
