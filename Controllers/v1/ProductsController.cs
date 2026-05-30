using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using System.Collections;

namespace ProductManagement.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            var response = products.Select(p => MapToDto(p));
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound($"Product {id} not found.");
            return Ok(MapToDto(product));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductCreateDto dto)
        {
            var domainModel = new Product
            {
                ProductName = dto.ProductName,
                ProductPrice = dto.ProductPrice,
                CreatedBy = dto.CreatedBy
            };
            var result = await _productService.AddAsync(domainModel);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId, version = "1.0" }, MapToDto(result));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ProductCreateDto dto)
        {
            var domainModel = new Product
            {
                ProductId = id,
                ProductName = dto.ProductName,
                ProductPrice = dto.ProductPrice,
                CreatedBy = dto.CreatedBy
            };
            return await _productService.EditAsync(id, domainModel) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _productService.DeleteAsync(id) ? NoContent() : NotFound();

        private static ProductV1ResponseDto MapToDto(Product p) => new()
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductPrice = p.ProductPrice,
            CreatedBy = p.CreatedBy
        };
    }
}
