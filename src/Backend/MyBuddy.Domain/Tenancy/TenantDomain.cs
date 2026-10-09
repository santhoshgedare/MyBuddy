namespace MyBuddy.Domain.Tenancy;

public sealed class TenantDomain : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public required string Hostname { get; init; }
    public bool IsVerified { get; set; }
}
