using System.Data;
using System.Net;
using Backend.Application.Invoices;
using Backend.Infrastructure.Api;
using Backend.Infrastructure.Pdf;
using Backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Backend.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(
    IInvoicesRepository invoices,
    ISettingsRepository settings,
    IPdfService pdf,
    IValidator<InvoiceCreateDto> createValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? invoiceNo,
        [FromQuery] string? customer,
        [FromQuery] string? mobile,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var res = await invoices.ListAsync(invoiceNo, customer, mobile, page, pageSize, cancellationToken);
        return Ok(res);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var details = await invoices.GetByIdAsync(id, cancellationToken);
        return details is null ? NotFound() : Ok(details);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InvoiceCreateDto dto, CancellationToken cancellationToken)
    {
        var validation = await createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            throw new ApiException(HttpStatusCode.BadRequest, "Validation failed.", "validation", validation.Errors.ToProblemDetails());

        var table = CreateInvoiceItemsDataTable(dto.Items);
        var (invoiceId, invoiceNo) = await invoices.CreateAsync(
            dto.CustomerId,
            dto.CustomerName,
            dto.Mobile,
            dto.DiscountType,
            dto.DiscountValue,
            dto.PaidAmount,
            dto.PaymentMode,
            dto.Notes,
            table,
            cancellationToken);

        return Ok(new InvoiceCreateResultDto(invoiceId, invoiceNo));
    }

    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> Pdf(int id, CancellationToken cancellationToken)
    {
        var details = await invoices.GetByIdAsync(id, cancellationToken);
        if (details is null) return NotFound();

        var setting = await settings.GetAsync(cancellationToken);
        var html = InvoiceHtmlRenderer.Render(setting, details.Invoice, details.Items);
        var bytes = await pdf.HtmlToPdfAsync(html, cancellationToken);
        return File(bytes, "application/pdf", $"{details.Invoice.InvoiceNo}.pdf");
    }

    [HttpGet("{id:int}/html")]
    public async Task<IActionResult> Html(int id, CancellationToken cancellationToken)
    {
        var details = await invoices.GetByIdAsync(id, cancellationToken);
        if (details is null) return NotFound();

        var setting = await settings.GetAsync(cancellationToken);
        var html = InvoiceHtmlRenderer.Render(setting, details.Invoice, details.Items);
        return Content(html, "text/html");
    }

    private static DataTable CreateInvoiceItemsDataTable(IReadOnlyList<InvoiceItemInputDto> items)
    {
        var table = new DataTable();
        table.Columns.Add("ProductId", typeof(int));
        table.Columns.Add("ProductName", typeof(string));
        table.Columns.Add("ProductType", typeof(string));
        table.Columns.Add("ModelNo", typeof(string));
        table.Columns.Add("SerialNo", typeof(string));
        table.Columns.Add("WarrantyMonths", typeof(int));
        table.Columns.Add("WarrantyNote", typeof(string));
        table.Columns.Add("Qty", typeof(decimal));
        table.Columns.Add("Rate", typeof(decimal));
        table.Columns.Add("DiscountType", typeof(string));
        table.Columns.Add("DiscountValue", typeof(decimal));
        table.Columns.Add("InvoiceNote", typeof(string));

        foreach (var it in items)
        {
            var row = table.NewRow();
            row["ProductId"] = it.ProductId ?? (object)DBNull.Value;
            row["ProductName"] = it.ProductName;
            row["ProductType"] = it.ProductType ?? (object)DBNull.Value;
            row["ModelNo"] = it.ModelNo ?? (object)DBNull.Value;
            row["SerialNo"] = it.SerialNo ?? (object)DBNull.Value;
            row["WarrantyMonths"] = it.WarrantyMonths ?? (object)DBNull.Value;
            row["WarrantyNote"] = it.WarrantyNote ?? (object)DBNull.Value;
            row["Qty"] = it.Qty;
            row["Rate"] = it.Rate;
            row["DiscountType"] = it.DiscountType ?? (object)DBNull.Value;
            row["DiscountValue"] = it.DiscountValue ?? (object)DBNull.Value;
            row["InvoiceNote"] = it.InvoiceNote ?? (object)DBNull.Value;
            table.Rows.Add(row);
        }

        return table;
    }
}

