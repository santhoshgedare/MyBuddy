namespace MyBuddy.Domain.Tenancy;

public sealed class TenantModuleSetting : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public required string ModuleCode { get; init; }
    public bool IsEnabled { get; set; }
}
