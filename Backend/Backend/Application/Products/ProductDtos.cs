namespace Backend.Application.Products;

public sealed record ProductCreateDto(
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
    decimal StockQty,
    string? DiscountType,
    decimal? DiscountValue,
    bool IsActive = true
);

public sealed record ProductUpdateDto(
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
    decimal StockQty,
    string? DiscountType,
    decimal? DiscountValue,
    bool IsActive
);

