namespace MyBuddy.Domain.Tenancy;

public sealed class Tenant
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public bool IsActive { get; set; } = true;
}
