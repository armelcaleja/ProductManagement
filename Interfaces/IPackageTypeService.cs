using ProductManagement.DTOs;
using ProductManagement.Model;

namespace ProductManagement.Interfaces
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
