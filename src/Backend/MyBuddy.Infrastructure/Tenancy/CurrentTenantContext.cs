using MyBuddy.Application.Tenancy;

namespace MyBuddy.Infrastructure.Tenancy;

public sealed class CurrentTenantContext : IActiveTenantContext
{
    public Guid? TenantId { get; private set; }

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("A valid tenant ID is required.", nameof(tenantId));
        }

        if (TenantId is not null && TenantId != tenantId)
        {
            throw new InvalidOperationException("The active tenant cannot change during a request.");
        }

        TenantId = tenantId;
    }
}
