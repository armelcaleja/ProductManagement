using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Services
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;
        public ItemService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Item>> GetAllAsync() => await _context.Items.ToListAsync();

        public async Task<Item?> GetByIdAsync(int id) => await _context.Items.FindAsync(id);

        public async Task<Item> AddAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> AssignToPackageAsync(PackageItem mapping)
        {
            _context.PackageItems.Add(mapping);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditAsync(int id, Item item)
        {
            var existing = await _context.Items.FindAsync(id);
            if (existing == null) return false;

            existing.ItemName = item.ItemName;
            existing.CreatedBy = item.CreatedBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Items.FindAsync(id);
            if (existing == null) return false;

            _context.Items.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
