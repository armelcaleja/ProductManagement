using ProductManagement.DTOs;
using ProductManagement.Model;

namespace ProductManagement.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item?> GetByIdAsync(int id);
        Task<Item> AddAsync(Item item);
        Task<bool> AssignToPackageAsync(PackageItem mapping);
        Task<bool> EditAsync(int id, Item item);
        Task<bool> DeleteAsync(int id);
    }
}
