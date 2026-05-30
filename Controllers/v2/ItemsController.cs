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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(int id, [FromBody] ItemCreateDto dto)
    {
        var domain = new Item { ItemId = id, ItemName = dto.ItemName, CreatedBy = GetUserId() };
        return await _service.EditAsync(id, domain) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();

    public static ItemResponseDto MapToDto(Item i) => new()
    {
        ItemId = i.ItemId,
        ItemName = i.ItemName
    };
}
