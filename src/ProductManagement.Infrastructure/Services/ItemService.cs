using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;

namespace ProductManagement.Infrastructure.Services
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLog;

        public ItemService(AppDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        public async Task<IEnumerable<Item>> GetAllAsync() => await _context.Items.ToListAsync();

        public async Task<Item?> GetByIdAsync(int id) => await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id);

        public async Task<Item> AddAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Add", "Item", item.ItemId,
                $"Added item '{item.ItemName}'", item.CreatedBy);
            return item;
        }

        public async Task<bool> AssignToPackageAsync(PackageItem mapping)
        {
            _context.PackageItems.Add(mapping);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                await _auditLog.LogAsync("Assign", "PackageItem", mapping.PackageItemId,
                    $"Assigned ItemId {mapping.ItemId} to PackageId {mapping.PackageId}", mapping.CreatedBy);
            }
            return result;
        }

        public async Task<bool> UnassignFromPackageAsync(int packageId, int itemId)
        {
            var existing = await _context.PackageItems
                .FirstOrDefaultAsync(pi => pi.PackageId == packageId && pi.ItemId == itemId);
            if (existing == null) return false;

            existing.IsDeleted = true;
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Unassign", "PackageItem", existing.PackageItemId,
                $"Unassigned ItemId {itemId} from PackageId {packageId}", existing.CreatedBy);
            return true;
        }

        public async Task<bool> EditAsync(int id, Item item)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null) return false;

            existing.ItemName = item.ItemName;
            existing.CreatedBy = item.CreatedBy;

            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Edit", "Item", id,
                $"Updated item to '{item.ItemName}'", item.CreatedBy);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true;
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Delete", "Item", id,
                $"Soft-deleted item '{existing.ItemName}'", existing.CreatedBy);
            return true;
        }
    }
}
