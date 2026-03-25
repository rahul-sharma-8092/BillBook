namespace Backend.Domain.Entities;

public sealed record Product(
    int Id,
    int? CategoryId,
    string ProductType,
    string Name,
    string? ModelNo,
    string? SerialNo,
    int? WarrantyMonths,
    string? WarrantyNote,
    string? InvoiceNote,
    decimal? PurchasePrice,
    decimal SellPrice,
    decimal? StockQty,
    string? DiscountType,
    decimal? DiscountValue,
    bool? IsActive,
    DateTime? CreatedAt,
    string? CategoryName = null
);

