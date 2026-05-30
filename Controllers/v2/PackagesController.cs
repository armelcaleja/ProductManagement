using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using System.Security.Claims;

namespace ProductManagement.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class PackagesController : ControllerBase
{
    private readonly IPackageService _service;
    public PackagesController(IPackageService service) => _service = service;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var packages = await _service.GetAllAsync();
        return Ok(packages.Select(p => MapToDto(p)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pkg = await _service.GetByIdAsync(id);
        return pkg == null ? NotFound() : Ok(MapToDto(pkg));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PackageCreateDto dto)
    {
        var domain = new Package
        {
            ProductId = dto.ProductId,
            ParentPackageId = dto.ParentPackageId,
            PackageTypeId = dto.PackageTypeId,
            CreatedBy = GetUserId()
        };
        var result = await _service.AddAsync(domain);
        return CreatedAtAction(nameof(GetById), new { id = result.PackageId }, MapToDto(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(int id, [FromBody] PackageCreateDto dto)
    {
        var domain = new Package
        {
            PackageId = id,
            ProductId = dto.ProductId,
            ParentPackageId = dto.ParentPackageId,
            PackageTypeId = dto.PackageTypeId,
            CreatedBy = GetUserId()
        };
        return await _service.EditAsync(id, domain) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();

    public static PackageResponseDto MapToDto(Package p) => new()
    {
        PackageId = p.PackageId,
        ProductId = p.ProductId,
        ParentPackageId = p.ParentPackageId,
        PackageTypeId = p.PackageTypeId,
        PackageTypeName = p.PackageType?.PackageTypeName ?? string.Empty,
        Items = p.Items.Select(i => ItemsController.MapToDto(i)).ToList(),
        PackageList = p.PackageList.Select(sub => MapToDto(sub)).ToList()
    };
}
