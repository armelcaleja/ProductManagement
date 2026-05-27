using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            var rawProducts = _context.Products.ToList();

            var productDtos = rawProducts.Select(p => new ProductDto { 
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice
            });

            return productDtos;
        }
    }
}
