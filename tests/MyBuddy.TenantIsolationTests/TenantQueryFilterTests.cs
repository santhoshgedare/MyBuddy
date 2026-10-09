using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyBuddy.Domain.Tenancy;
using MyBuddy.Infrastructure.Tenancy;
using MyBuddy.Infrastructure.Identity;
using MyBuddy.Infrastructure.Persistence;

namespace MyBuddy.TenantIsolationTests;

public class TenantQueryFilterTests
{
    [Fact]
    public async Task Tenant_owned_rows_are_scoped_and_hidden_without_active_tenant()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var tenantA = new Tenant { Name = "Company A", Slug = "company-a" };
        var tenantB = new Tenant { Name = "Company B", Slug = "company-b" };
        var user = new MyBuddyUser { Id = Guid.NewGuid(), UserName = "employee@example.test" };
        var membershipB = new TenantMembership { TenantId = tenantB.Id, UserId = user.Id };

        await using (var seed = CreateContext(connection, new CurrentTenantContext()))
        {
            await seed.Database.EnsureCreatedAsync();
            seed.Tenants.AddRange(tenantA, tenantB);
            seed.Users.Add(user);
            seed.TenantMemberships.AddRange(
                new TenantMembership { TenantId = tenantA.Id, UserId = user.Id },
                membershipB);
            await seed.SaveChangesAsync();
        }

        await using (var unscoped = CreateContext(connection, new CurrentTenantContext()))
        {
            Assert.Empty(await unscoped.TenantMemberships.ToListAsync());
        }

        var tenantContext = new CurrentTenantContext();
        tenantContext.SetTenant(tenantA.Id);
        await using var scoped = CreateContext(connection, tenantContext);

        Assert.Equal(
            tenantA.Id,
            Assert.Single(await scoped.TenantMemberships.ToListAsync()).TenantId);
        Assert.Empty(await scoped.TenantMemberships
            .Where(membership => membership.Id == membershipB.Id)
            .ToListAsync());
    }

    [Fact]
    public async Task Membership_reader_checks_both_user_and_requested_tenant()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var tenantA = new Tenant { Name = "Company A", Slug = "company-a" };
        var tenantB = new Tenant { Name = "Company B", Slug = "company-b" };
        var member = new MyBuddyUser { Id = Guid.NewGuid(), UserName = "member@example.test" };
        var outsider = new MyBuddyUser { Id = Guid.NewGuid(), UserName = "outsider@example.test" };

        await using (var seed = CreateContext(connection, new CurrentTenantContext()))
        {
            await seed.Database.EnsureCreatedAsync();
            seed.Tenants.AddRange(tenantA, tenantB);
            seed.Users.AddRange(member, outsider);
            seed.TenantMemberships.AddRange(
                new TenantMembership { TenantId = tenantA.Id, UserId = member.Id },
                new TenantMembership { TenantId = tenantB.Id, UserId = member.Id },
                new TenantMembership
                {
                    TenantId = tenantB.Id,
                    UserId = outsider.Id,
                    Status = MembershipStatus.Invited
                });
            await seed.SaveChangesAsync();
        }

        var tenantContext = new CurrentTenantContext();
        tenantContext.SetTenant(tenantA.Id);
        await using var scoped = CreateContext(connection, tenantContext);
        var membershipReader = new EfTenantMembershipReader(scoped);

        Assert.True(await membershipReader.HasActiveMembershipAsync(member.Id, tenantB.Id));
        Assert.False(await membershipReader.HasActiveMembershipAsync(outsider.Id, tenantB.Id));
        Assert.False(await membershipReader.HasActiveMembershipAsync(member.Id, Guid.NewGuid()));
    }

    private static MyBuddyDbContext CreateContext(
        SqliteConnection connection,
        CurrentTenantContext tenantContext) =>
        new(new DbContextOptionsBuilder<MyBuddyDbContext>()
            .UseSqlite(connection)
            .Options, tenantContext);
}
