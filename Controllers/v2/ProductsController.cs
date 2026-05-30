using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Controllers.V2 
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            var response = products.Select(p => MapToV2Dto(p));
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound($"Product {id} not found.");
            return Ok(MapToV2Dto(product));
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
            var result = await _service.AddAsync(domainModel);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId, version = "2.0" }, MapToV2Dto(result));
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
            return await _service.EditAsync(id, domainModel) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();

        private static ProductV2ResponseDto MapToV2Dto(Product p) => new()
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductPrice = p.ProductPrice,
            Packages = p.Packages.Select(pkg => PackagesController.MapToDto(pkg)).ToList()
        };
    }
}

