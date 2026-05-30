using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Services
{
    public class PackageTypeService : IPackageTypeService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLog;

        public PackageTypeService(AppDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        public async Task<IEnumerable<PackageType>> GetAllAsync() => await _context.PackageTypes.ToListAsync();

        public async Task<PackageType?> GetByIdAsync(int id) => await _context.PackageTypes.FirstOrDefaultAsync(pt => pt.PackageTypeId == id);

        public async Task<PackageType> AddAsync(PackageType packageType)
        {
            _context.PackageTypes.Add(packageType);
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Add", "PackageType", packageType.PackageTypeId,
                $"Added package type '{packageType.PackageTypeName}'", packageType.CreatedBy);
            return packageType;
        }

        public async Task<bool> EditAsync(int id, PackageType packageType)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null) return false;

            existing.PackageTypeName = packageType.PackageTypeName;
            existing.CreatedBy = packageType.CreatedBy;

            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Edit", "PackageType", id,
                $"Updated package type to '{packageType.PackageTypeName}'", packageType.CreatedBy);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true;
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Delete", "PackageType", id,
                $"Soft-deleted package type '{existing.PackageTypeName}'", existing.CreatedBy);
            return true;
        }
    }
}
