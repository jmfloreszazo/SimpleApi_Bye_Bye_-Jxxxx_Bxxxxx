namespace SimpleApi.Vanilla;

public sealed class VanillaValidationException(Dictionary<string, string[]> errors) : Exception("Validation failed")
{
    public Dictionary<string, string[]> Errors { get; } = errors;
}
