namespace MyBuddy.Domain.Tenancy;

public sealed class TenantRole : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public required string Name { get; init; }
}
