namespace kiwiDeal.SharedKernel.Outbox;

public sealed class OutboxTypeRegistry : IOutboxTypeRegistry
{
    private readonly Dictionary<string, Type> _types = new();

    public void Register<T>() where T : class
    {
        var type = typeof(T);
        _types[type.FullName!] = type;
    }

    public Type? Resolve(string fullTypeName)
    {
        _types.TryGetValue(fullTypeName, out var type);
        return type;
    }
}
