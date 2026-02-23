namespace kiwiDeal.SharedKernel.Outbox;

public interface IOutboxTypeRegistry
{
    void Register<T>() where T : class;
    Type? Resolve(string fullTypeName);
}
