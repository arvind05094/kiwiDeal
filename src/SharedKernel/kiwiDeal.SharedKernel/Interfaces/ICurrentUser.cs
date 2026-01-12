namespace kiwiDeal.SharedKernel.Interfaces;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    string? Name { get; }
    bool IsAuthenticated { get; }
}
