using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Interfaces
{
    public interface IPackageTypeService
    {
        Task<IEnumerable<PackageType>> GetAllAsync();
        Task<PackageType?> GetByIdAsync(int id);
        Task<PackageType> AddAsync(PackageType packageType);
        Task<bool> EditAsync(int id, PackageType packageType);
        Task<bool> DeleteAsync(int id);
    }
}
