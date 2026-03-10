namespace SimpleApi.Vanilla;

public sealed class VanillaValidationResult
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool IsValid => _errors.Count == 0;

    public VanillaValidationResult AddError(string property, string message)
    {
        if (!_errors.TryGetValue(property, out var list))
            _errors[property] = list = [];
        list.Add(message);
        return this;
    }

    public Dictionary<string, string[]> ToDictionary()
        => _errors.ToDictionary(k => k.Key, v => v.Value.ToArray());
}
