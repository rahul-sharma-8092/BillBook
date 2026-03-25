namespace Backend.Domain.Entities;

public sealed record Invoice(
    int Id,
    string InvoiceNo,
    int? CustomerId,
    string? CustomerName,
    string? Mobile,
    decimal SubTotal,
    string? DiscountType,
    decimal? DiscountValue,
    decimal? DiscountAmount,
    decimal TotalAmount,
    decimal? PaidAmount,
    decimal? BalanceAmount,
    string? PaymentMode,
    string? Notes,
    DateTime? CreatedAt
);

