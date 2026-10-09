namespace MyBuddy.Domain.Tenancy;

public sealed class TenantSubscription : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public required string PlanCode { get; init; }
    public DateTimeOffset StartsAt { get; init; }
    public DateTimeOffset? EndsAt { get; init; }
}
