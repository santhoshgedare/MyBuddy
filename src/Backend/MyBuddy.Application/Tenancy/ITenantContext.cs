namespace MyBuddy.Application.Tenancy;

public interface ITenantContext
{
    Guid? TenantId { get; }
}
