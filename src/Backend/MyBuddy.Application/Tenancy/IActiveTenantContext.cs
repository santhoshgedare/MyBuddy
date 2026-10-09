namespace MyBuddy.Application.Tenancy;

public interface IActiveTenantContext : ITenantContext
{
    void SetTenant(Guid tenantId);
}
