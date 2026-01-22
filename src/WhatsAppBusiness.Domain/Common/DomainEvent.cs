namespace WhatsAppBusiness.Domain.Common;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}
