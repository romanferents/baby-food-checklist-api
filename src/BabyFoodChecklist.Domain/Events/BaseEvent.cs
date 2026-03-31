namespace BabyFoodChecklist.Domain.Events;

public abstract class BaseEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
