using MyBuddy.Application.Tenancy;
using MyBuddy.Infrastructure.Tenancy;

namespace MyBuddy.UnitTests;

public class ActiveTenantResolverTests
{
    [Fact]
    public async Task Sets_tenant_only_when_authenticated_user_has_active_membership()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var context = new CurrentTenantContext();
        var resolver = new ActiveTenantResolver(
            new StubMembershipReader(userId, tenantId),
            context);

        var resolved = await resolver.TrySetActiveTenantAsync(userId, tenantId);

        Assert.True(resolved);
        Assert.Equal(tenantId, context.TenantId);
    }

    [Fact]
    public async Task Rejects_user_without_active_membership()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var context = new CurrentTenantContext();
        var resolver = new ActiveTenantResolver(
            new StubMembershipReader(userId, tenantId, hasMembership: false),
            context);

        var resolved = await resolver.TrySetActiveTenantAsync(userId, tenantId);

        Assert.False(resolved);
        Assert.Null(context.TenantId);
    }

    [Fact]
    public async Task Rejects_membership_belonging_to_a_different_user()
    {
        var context = new CurrentTenantContext();
        var resolver = new ActiveTenantResolver(
            new StubMembershipReader(Guid.NewGuid(), Guid.NewGuid()),
            context);

        var resolved = await resolver.TrySetActiveTenantAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(resolved);
        Assert.Null(context.TenantId);
    }

    [Fact]
    public async Task Rejects_empty_authenticated_user_id()
    {
        var tenantId = Guid.NewGuid();
        var context = new CurrentTenantContext();
        var resolver = new ActiveTenantResolver(
            new StubMembershipReader(Guid.Empty, tenantId),
            context);

        var resolved = await resolver.TrySetActiveTenantAsync(Guid.Empty, tenantId);

        Assert.False(resolved);
        Assert.Null(context.TenantId);
    }

    private sealed class StubMembershipReader(
        Guid userId,
        Guid tenantId,
        bool hasMembership = true) : ITenantMembershipReader
    {
        public Task<bool> HasActiveMembershipAsync(
            Guid requestedUserId,
            Guid requestedTenantId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(
                hasMembership &&
                requestedUserId == userId &&
                requestedTenantId == tenantId);
    }
}
