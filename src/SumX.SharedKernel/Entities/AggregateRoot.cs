namespace SumX.SharedKernel.Entities;

public abstract class AggregateRoot : BaseEntity
{
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }
}
