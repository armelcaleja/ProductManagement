using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Interfaces
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
