using ProductManagement.DTOs;
using ProductManagement.Model;

namespace ProductManagement.Interfaces
{
    public interface IProductService
    {
        public IEnumerable<ProductDto> GetAllProducts();
    }
}
