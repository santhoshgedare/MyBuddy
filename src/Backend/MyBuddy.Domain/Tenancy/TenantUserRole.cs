namespace MyBuddy.Domain.Tenancy;

public sealed class TenantUserRole : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public Guid TenantMembershipId { get; init; }
    public Guid TenantRoleId { get; init; }
}
