namespace Backend.Application.Categories;

public sealed record CategoryCreateDto(string Name, bool IsActive = true);
public sealed record CategoryUpdateDto(string Name, bool IsActive);

