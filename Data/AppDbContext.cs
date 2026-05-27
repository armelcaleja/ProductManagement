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

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)  
        {
        
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
