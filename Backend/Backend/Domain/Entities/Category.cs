namespace Backend.Domain.Entities;

public sealed record Category(int Id, string Name, bool? IsActive, DateTime? CreatedAt);

