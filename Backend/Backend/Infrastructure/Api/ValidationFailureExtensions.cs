using FluentValidation.Results;

namespace Backend.Infrastructure.Api;

public static class ValidationFailureExtensions
{
    public static object ToProblemDetails(this IEnumerable<ValidationFailure> failures)
    {
        return failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}

