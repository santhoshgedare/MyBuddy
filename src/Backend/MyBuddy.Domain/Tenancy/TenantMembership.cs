namespace MyBuddy.Domain.Tenancy;

public sealed class TenantMembership : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
}

public enum MembershipStatus
{
    Invited,
    Active,
    Suspended,
    Revoked
}
