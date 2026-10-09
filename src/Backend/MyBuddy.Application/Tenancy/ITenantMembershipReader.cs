namespace MyBuddy.Application.Tenancy;

public interface ITenantMembershipReader
{
    Task<bool> HasActiveMembershipAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
