namespace SimpleApi.Vanilla;

/// <summary>
/// Zero-dependency validator interface. Replaces FluentValidation completely.
/// </summary>
public interface IVanillaValidator<in T>
{
    VanillaValidationResult Validate(T instance);
}
