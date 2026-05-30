using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Services
{
    public class PackageTypeService : IPackageTypeService
    {
        private readonly AppDbContext _context;
        public PackageTypeService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<PackageType>> GetAllAsync() => await _context.PackageTypes.ToListAsync();

        public async Task<PackageType?> GetByIdAsync(int id) => await _context.PackageTypes.FindAsync(id);

        public async Task<PackageType> AddAsync(PackageType packageType)
        {
            _context.PackageTypes.Add(packageType);
            await _context.SaveChangesAsync();
            return packageType;
        }

        public async Task<bool> EditAsync(int id, PackageType packageType)
        {
            var existing = await _context.PackageTypes.FindAsync(id);
            if (existing == null) return false;

            existing.PackageTypeName = packageType.PackageTypeName;
            existing.CreatedBy = packageType.CreatedBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.PackageTypes.FindAsync(id);
            if (existing == null) return false;

            _context.PackageTypes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
