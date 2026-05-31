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
public class ItemsController : ControllerBase
{
    private readonly IItemService _service;
    public ItemsController(IItemService service) => _service = service;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items.Select(i => MapToDto(i)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(MapToDto(item));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ItemCreateDto dto)
    {
        var domain = new Item { ItemName = dto.ItemName, CreatedBy = GetUserId() };
        var result = await _service.AddAsync(domain);
        return CreatedAtAction(nameof(GetById), new { id = result.ItemId }, MapToDto(result));
    }

    [HttpPost("assign-to-package")]
    public async Task<IActionResult> AssignToPackage([FromBody] PackageItemAssignDto dto)
    {
        var domainMapping = new PackageItem
        {
            PackageId = dto.PackageId,
            ItemId = dto.ItemId,
            CreatedBy = GetUserId()
        };
        return await _service.AssignToPackageAsync(domainMapping) ? Ok("Assigned successfully.") : BadRequest();
    }

    [HttpDelete("unassign-from-package")]
    public async Task<IActionResult> UnassignFromPackage([FromBody] PackageItemAssignDto dto)
    {
        var result = await _service.UnassignFromPackageAsync(dto.PackageId, dto.ItemId);
        if (!result) return NotFound(new { message = "Package-Item assignment not found." });
        return Ok(new { message = "Item successfully unassigned from package." });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(int id, [FromBody] ItemCreateDto dto)
    {
        var domain = new Item { ItemId = id, ItemName = dto.ItemName, CreatedBy = GetUserId() };
        var result = await _service.EditAsync(id, domain);
        if (!result) return NotFound(new { message = $"Item with ID {id} not found." });
        return Ok(new { message = "Item successfully updated." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound(new { message = $"Item with ID {id} not found." });
        return Ok(new { message = "Item successfully deleted." });
    }

    public static ItemResponseDto MapToDto(Item i) => new()
    {
        ItemId = i.ItemId,
        ItemName = i.ItemName
    };
}
