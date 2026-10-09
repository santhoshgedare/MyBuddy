namespace MyBuddy.Domain.Tenancy;

public sealed class Permission
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Code { get; init; }
}
