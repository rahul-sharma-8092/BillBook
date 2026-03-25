namespace Backend.Domain.Entities;

public sealed record Setting(
    int Id,
    string? ShopName,
    string? Address,
    string? Phone,
    string? Email,
    string? BankName,
    string? AccountName,
    string? AccountNumber,
    string? IFSC,
    string? UPI,
    string? Terms,
    string? FooterMessage
);

