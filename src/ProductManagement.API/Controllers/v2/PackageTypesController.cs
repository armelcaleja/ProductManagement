using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using System.Security.Claims;

namespace ProductManagement.API.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class PackageTypesController : ControllerBase
{
    private readonly IPackageTypeService _service;
    public PackageTypesController(IPackageTypeService service) => _service = service;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _service.GetAllAsync();
        return Ok(types.Select(t => MapToDto(t)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var type = await _service.GetByIdAsync(id);
        return type == null ? NotFound() : Ok(MapToDto(type));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PackageTypeCreateDto dto)
    {
        var domain = new PackageType { PackageTypeName = dto.PackageTypeName, CreatedBy = GetUserId() };
        var result = await _service.AddAsync(domain);
        return CreatedAtAction(nameof(GetById), new { id = result.PackageTypeId }, MapToDto(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(int id, [FromBody] PackageTypeCreateDto dto)
    {
        var domain = new PackageType { PackageTypeId = id, PackageTypeName = dto.PackageTypeName, CreatedBy = GetUserId() };
        var result = await _service.EditAsync(id, domain);
        if (!result) return NotFound(new { message = $"PackageType with ID {id} not found." });
        return Ok(new { message = "PackageType successfully updated." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound(new { message = $"PackageType with ID {id} not found." });
        return Ok(new { message = "PackageType successfully deleted." });
    }

    private static PackageTypeResponseDto MapToDto(PackageType t) => new()
    {
        PackageTypeId = t.PackageTypeId,
        PackageTypeName = t.PackageTypeName
    };
}
