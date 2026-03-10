using SimpleApi.Pipeline;

namespace SimpleApi.Vanilla;

/// <summary>
/// Pipeline behavior that validates using our own IVanillaValidator.
/// Zero external dependencies — no FluentValidation.
/// </summary>
public sealed class VanillaValidationBehavior<TRequest, TResponse>(
    IEnumerable<IVanillaValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken ct = default)
    {
        var allErrors = new Dictionary<string, string[]>();

        foreach (var validator in validators)
        {
            var result = validator.Validate(request);
            if (!result.IsValid)
            {
                foreach (var kvp in result.ToDictionary())
                    allErrors[kvp.Key] = kvp.Value;
            }
        }

        if (allErrors.Count > 0)
            throw new VanillaValidationException(allErrors);

        return await next();
    }
}
