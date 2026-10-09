namespace MyBuddy.Domain.Tenancy;

public sealed class TenantIdentityProvider : ITenantOwned
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public required string Authority { get; init; }
    public required string ClientId { get; init; }
    public bool IsEnabled { get; set; }
}
