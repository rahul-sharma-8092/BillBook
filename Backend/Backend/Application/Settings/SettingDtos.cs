namespace Backend.Application.Settings;

public sealed record SettingUpsertDto(
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

