using System.Net;
using Backend.Application.Categories;
using Backend.Infrastructure.Api;
using Backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ICategoriesRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool onlyActive, CancellationToken cancellationToken)
    {
        var items = await repo.ListAsync(onlyActive, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await repo.GetAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ApiException(HttpStatusCode.BadRequest, "Name is required.", "validation");

        var id = await repo.CreateAsync(dto.Name.Trim(), dto.IsActive, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ApiException(HttpStatusCode.BadRequest, "Name is required.", "validation");

        var ok = await repo.UpdateAsync(id, dto.Name.Trim(), dto.IsActive, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await repo.DeleteAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}

