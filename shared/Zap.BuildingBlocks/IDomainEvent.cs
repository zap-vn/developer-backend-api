namespace Zap.BuildingBlocks;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
