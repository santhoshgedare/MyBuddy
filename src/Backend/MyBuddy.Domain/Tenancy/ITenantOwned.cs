namespace MyBuddy.Domain.Tenancy;

public interface ITenantOwned
{
    Guid Id { get; }
    Guid TenantId { get; }
}
