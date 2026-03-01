using kiwiDeal.SharedKernel.Interfaces;

namespace kiwiDeal.Payments.Domain.Entities;

public record PaymentId : IStronglyTypedId
{
    public Guid Value { get; }
    private PaymentId(Guid value) { Value = value; }
    public static PaymentId New() => new(Guid.CreateVersion7());
    public static PaymentId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
