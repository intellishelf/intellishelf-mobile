namespace Intellishelf.Common.TryResult;

public record ProblemDetails
{
    public string? Title { get; init; }

    public Dictionary<string, string[]>? Errors { get; init; }
}