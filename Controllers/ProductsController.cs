using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using System.Collections;

namespace ProductManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IEnumerable<ProductDto> GetAllProducts()
        {
            var products = _productService.GetAllProducts();

            return products;
        }
    }
}
