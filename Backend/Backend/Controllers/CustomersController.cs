using System.Net;
using Backend.Application.Customers;
using Backend.Infrastructure.Api;
using Backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ICustomersRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var items = await repo.ListAsync(q, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await repo.GetAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ApiException(HttpStatusCode.BadRequest, "Name is required.", "validation");

        var id = await repo.CreateAsync(new
        {
            Name = dto.Name.Trim(),
            dto.CompanyName,
            dto.Mobile,
            dto.Email,
            dto.AddressLine1,
            dto.AddressLine2,
            dto.City,
            dto.District,
            dto.State,
            dto.Country,
            dto.Notes
        }, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ApiException(HttpStatusCode.BadRequest, "Name is required.", "validation");

        var ok = await repo.UpdateAsync(new
        {
            Id = id,
            Name = dto.Name.Trim(),
            dto.CompanyName,
            dto.Mobile,
            dto.Email,
            dto.AddressLine1,
            dto.AddressLine2,
            dto.City,
            dto.District,
            dto.State,
            dto.Country,
            dto.Notes
        }, cancellationToken);

        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await repo.DeleteAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}

