namespace Backend.Application.Invoices;

public sealed record InvoiceItemInputDto(
    int? ProductId,
    string ProductName,
    string? ProductType,
    string? ModelNo,
    string? SerialNo,
    int? WarrantyMonths,
    string? WarrantyNote,
    decimal Qty,
    decimal Rate,
    string? DiscountType,
    decimal? DiscountValue,
    string? InvoiceNote
);

public sealed record InvoiceCreateDto(
    int? CustomerId,
    string? CustomerName,
    string? Mobile,
    string? DiscountType,
    decimal? DiscountValue,
    decimal PaidAmount,
    string? PaymentMode,
    string? Notes,
    List<InvoiceItemInputDto> Items
);

public sealed record InvoiceCreateResultDto(int InvoiceId, string InvoiceNo);

