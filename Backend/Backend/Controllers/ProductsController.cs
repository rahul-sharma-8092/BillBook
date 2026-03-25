using System.Net;
using Backend.Application.Products;
using Backend.Infrastructure.Api;
using Backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductsRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? q, [FromQuery] int? categoryId, [FromQuery] bool onlyActive, CancellationToken cancellationToken)
    {
        var items = await repo.ListAsync(q, categoryId, onlyActive, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await repo.GetAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ApiException(HttpStatusCode.BadRequest, "Name is required.", "validation");
        if (string.IsNullOrWhiteSpace(dto.ProductType))
            throw new ApiException(HttpStatusCode.BadRequest, "ProductType is required.", "validation");

        var id = await repo.CreateAsync(new
        {
            dto.CategoryId,
            dto.ProductType,
            Name = dto.Name.Trim(),
            dto.ModelNo,
            dto.SerialNo,
            dto.WarrantyMonths,
            dto.WarrantyNote,
            dto.InvoiceNote,
            dto.PurchasePrice,
            dto.SellPrice,
            dto.StockQty,
            dto.DiscountType,
            dto.DiscountValue,
            dto.IsActive
        }, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ApiException(HttpStatusCode.BadRequest, "Name is required.", "validation");
        if (string.IsNullOrWhiteSpace(dto.ProductType))
            throw new ApiException(HttpStatusCode.BadRequest, "ProductType is required.", "validation");

        var ok = await repo.UpdateAsync(new
        {
            Id = id,
            dto.CategoryId,
            dto.ProductType,
            Name = dto.Name.Trim(),
            dto.ModelNo,
            dto.SerialNo,
            dto.WarrantyMonths,
            dto.WarrantyNote,
            dto.InvoiceNote,
            dto.PurchasePrice,
            dto.SellPrice,
            dto.StockQty,
            dto.DiscountType,
            dto.DiscountValue,
            dto.IsActive
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

