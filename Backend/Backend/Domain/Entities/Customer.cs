namespace Backend.Domain.Entities;

public sealed record Customer(
    int Id,
    string Name,
    string? CompanyName,
    string? Mobile,
    string? Email,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? District,
    string? State,
    string? Country,
    string? Notes,
    DateTime? CreatedAt
);

