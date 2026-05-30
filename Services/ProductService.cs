using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLog;

        public ProductService(AppDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products
                .Include(p => p.Packages).ThenInclude(pkg => pkg.PackageType)
                .Include(p => p.Packages).ThenInclude(pkg => pkg.Items)
                .Include(p => p.Packages).ThenInclude(pkg => pkg.PackageList).ThenInclude(sub => sub.PackageType)
                .Include(p => p.Packages).ThenInclude(pkg => pkg.PackageList).ThenInclude(sub => sub.Items)
                .ToListAsync();

        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products
                .Include(p => p.Packages).ThenInclude(pkg => pkg.PackageType)
                .Include(p => p.Packages).ThenInclude(pkg => pkg.Items)
                .Include(p => p.Packages).ThenInclude(pkg => pkg.PackageList).ThenInclude(sub => sub.PackageType)
                .Include(p => p.Packages).ThenInclude(pkg => pkg.PackageList).ThenInclude(sub => sub.Items)
                .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Add", "Product", product.ProductId,
                $"Added product '{product.ProductName}' with price {product.ProductPrice}", product.CreatedBy);
            return product;
        }

        public async Task<bool> EditAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;

            existing.ProductName = product.ProductName;
            existing.ProductPrice = product.ProductPrice;
            existing.CreatedBy = product.CreatedBy;

            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Edit", "Product", id,
                $"Updated product to '{product.ProductName}' with price {product.ProductPrice}", product.CreatedBy);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Delete", "Product", id,
                $"Deleted product '{existing.ProductName}'", existing.CreatedBy);
            return true;
        }
    }
}
