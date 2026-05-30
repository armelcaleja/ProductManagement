using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Services
{
    public class PackageService : IPackageService
    {
        private readonly AppDbContext _context;
        public PackageService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Package>> GetAllAsync() =>
            await _context.Packages.Include(p => p.Items).Include(p => p.PackageList).ToListAsync();

        public async Task<Package?> GetByIdAsync(int id) =>
            await _context.Packages.Include(p => p.Items).Include(p => p.PackageList).FirstOrDefaultAsync(p => p.PackageId == id);

        public async Task<Package> AddAsync(Package package)
        {
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task<bool> EditAsync(int id, Package package)
        {
            var existing = await _context.Packages.FindAsync(id);
            if (existing == null) return false;

            existing.ProductId = package.ProductId;
            existing.ParentPackageId = package.ParentPackageId;
            existing.PackageTypeId = package.PackageTypeId;
            existing.CreatedBy = package.CreatedBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Packages.FindAsync(id);
            if (existing == null) return false;

            _context.Packages.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
