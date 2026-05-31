using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;

namespace ProductManagement.Infrastructure.Services
{
    public class PackageService : IPackageService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLog;

        public PackageService(AppDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        public async Task<IEnumerable<Package>> GetAllAsync() =>
            await _context.Packages.Include(p => p.Items).Include(p => p.PackageList).ToListAsync();

        public async Task<Package?> GetByIdAsync(int id) =>
            await _context.Packages.Include(p => p.Items).Include(p => p.PackageList).FirstOrDefaultAsync(p => p.PackageId == id);

        private async Task<Package?> FindActiveAsync(int id) =>
            await _context.Packages.FirstOrDefaultAsync(p => p.PackageId == id);

        public async Task<Package> AddAsync(Package package)
        {
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Add", "Package", package.PackageId,
                $"Added package for ProductId {package.ProductId}, PackageTypeId {package.PackageTypeId}", package.CreatedBy);
            return package;
        }

        public async Task<bool> EditAsync(int id, Package package)
        {
            var existing = await FindActiveAsync(id);
            if (existing == null) return false;

            existing.ProductId = package.ProductId;
            existing.ParentPackageId = package.ParentPackageId;
            existing.PackageTypeId = package.PackageTypeId;
            existing.CreatedBy = package.CreatedBy;

            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Edit", "Package", id,
                $"Updated package: ProductId {package.ProductId}, PackageTypeId {package.PackageTypeId}", package.CreatedBy);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await FindActiveAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true;
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Delete", "Package", id,
                $"Soft-deleted package (ProductId {existing.ProductId})", existing.CreatedBy);
            return true;
        }
    }
}
