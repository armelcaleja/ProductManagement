using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Model;

namespace ProductManagement.Extensions
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                await context.Database.MigrateAsync();

                if (await context.Products.AnyAsync())
                    return;

                // 1. Seed PackageTypes
                var packageTypes = new List<PackageType>
                {
                    new() { PackageTypeName = "Box", CreatedBy = 1 },
                    new() { PackageTypeName = "Crate", CreatedBy = 1 },
                    new() { PackageTypeName = "Pallet", CreatedBy = 1 }
                };
                context.PackageTypes.AddRange(packageTypes);
                await context.SaveChangesAsync();

                // 2. Seed Products
                var products = new List<Product>
                {
                    new() { ProductName = "Product Alpha", ProductPrice = 100, CreatedBy = 1 },
                    new() { ProductName = "Product Beta", ProductPrice = 250, CreatedBy = 1 },
                    new() { ProductName = "Product Gamma", ProductPrice = 500, CreatedBy = 1 }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                // 3. Seed root Packages (ParentPackageId = null means no parent)
                var rootPackage1 = new Package
                {
                    ProductId = products[0].ProductId,
                    ParentPackageId = null,
                    PackageTypeId = packageTypes[0].PackageTypeId,
                    CreatedBy = 1
                };
                var rootPackage2 = new Package
                {
                    ProductId = products[1].ProductId,
                    ParentPackageId = null,
                    PackageTypeId = packageTypes[2].PackageTypeId,
                    CreatedBy = 1
                };
                context.Packages.AddRange(rootPackage1, rootPackage2);
                await context.SaveChangesAsync();

                // 4. Seed child Packages (referencing root packages as parent)
                var childPackage1 = new Package
                {
                    ProductId = products[0].ProductId,
                    ParentPackageId = rootPackage1.PackageId,
                    PackageTypeId = packageTypes[1].PackageTypeId,
                    CreatedBy = 1
                };
                var childPackage2 = new Package
                {
                    ProductId = products[0].ProductId,
                    ParentPackageId = rootPackage1.PackageId,
                    PackageTypeId = packageTypes[0].PackageTypeId,
                    CreatedBy = 1
                };
                var childPackage3 = new Package
                {
                    ProductId = products[1].ProductId,
                    ParentPackageId = rootPackage2.PackageId,
                    PackageTypeId = packageTypes[1].PackageTypeId,
                    CreatedBy = 1
                };
                context.Packages.AddRange(childPackage1, childPackage2, childPackage3);
                await context.SaveChangesAsync();

                // 5. Seed Items assigned to packages
                var items = new List<Item>
                {
                    new() { ItemName = "Widget A", CreatedBy = 1 },
                    new() { ItemName = "Widget B", CreatedBy = 1 },
                    new() { ItemName = "Gadget X", CreatedBy = 1 },
                    new() { ItemName = "Gadget Y", CreatedBy = 1 },
                    new() { ItemName = "Component Z", CreatedBy = 1 },
                    new() { ItemName = "Part W", CreatedBy = 1 }
                };

                // Assign items to packages via navigation
                rootPackage1.Items.Add(items[0]);
                rootPackage1.Items.Add(items[1]);
                childPackage1.Items.Add(items[2]);
                childPackage2.Items.Add(items[3]);
                rootPackage2.Items.Add(items[4]);
                childPackage3.Items.Add(items[5]);
                await context.SaveChangesAsync();

                // 6. Seed PackageItems
                var packageItems = new List<PackageItem>
                {
                    new() { PackageId = rootPackage1.PackageId, ItemId = items[0].ItemId, CreatedBy = 1 },
                    new() { PackageId = rootPackage1.PackageId, ItemId = items[1].ItemId, CreatedBy = 1 },
                    new() { PackageId = childPackage1.PackageId, ItemId = items[2].ItemId, CreatedBy = 1 },
                    new() { PackageId = childPackage2.PackageId, ItemId = items[3].ItemId, CreatedBy = 1 },
                    new() { PackageId = rootPackage2.PackageId, ItemId = items[4].ItemId, CreatedBy = 1 },
                    new() { PackageId = childPackage3.PackageId, ItemId = items[5].ItemId, CreatedBy = 1 }
                };
                context.PackageItems.AddRange(packageItems);
                await context.SaveChangesAsync();

                // 7. Seed User
                var user = new User
                {
                    Username = "armelcaleja",
                    Password = BCrypt.Net.BCrypt.HashPassword("asdasdasd")
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
