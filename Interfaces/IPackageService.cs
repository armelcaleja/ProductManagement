using ProductManagement.DTOs;
using ProductManagement.Model;

namespace ProductManagement.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<Package>> GetAllAsync();
        Task<Package?> GetByIdAsync(int id);
        Task<Package> AddAsync(Package package);
        Task<bool> EditAsync(int id, Package package);
        Task<bool> DeleteAsync(int id);
    }
}
