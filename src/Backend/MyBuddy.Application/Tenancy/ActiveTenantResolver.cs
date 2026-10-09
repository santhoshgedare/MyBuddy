namespace MyBuddy.Application.Tenancy;

public sealed class ActiveTenantResolver(
    ITenantMembershipReader memberships,
    IActiveTenantContext tenantContext)
{
    public async Task<bool> TrySetActiveTenantAsync(
        Guid authenticatedUserId,
        Guid requestedTenantId,
        CancellationToken cancellationToken = default)
    {
        if (authenticatedUserId == Guid.Empty || requestedTenantId == Guid.Empty)
        {
            return false;
        }

        if (!await memberships.HasActiveMembershipAsync(
                authenticatedUserId,
                requestedTenantId,
                cancellationToken))
        {
            return false;
        }

        tenantContext.SetTenant(requestedTenantId);
        return true;
    }
}
