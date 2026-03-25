namespace Backend.Domain.Entities;

public sealed record InvoiceItem(
    int Id,
    int InvoiceId,
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
    decimal? DiscountAmount,
    decimal Amount,
    string? InvoiceNote
);

