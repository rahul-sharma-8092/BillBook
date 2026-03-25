namespace Backend.Application.Customers;

public sealed record CustomerCreateDto(
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
    string? Notes
);

public sealed record CustomerUpdateDto(
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
    string? Notes
);

