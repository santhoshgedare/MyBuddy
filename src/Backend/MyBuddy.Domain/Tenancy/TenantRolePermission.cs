namespace MyBuddy.Domain.Tenancy;

public sealed class TenantRolePermission : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public Guid TenantRoleId { get; init; }
    public Guid PermissionId { get; init; }
}
