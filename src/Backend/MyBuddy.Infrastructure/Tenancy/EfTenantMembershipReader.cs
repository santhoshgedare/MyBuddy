using Microsoft.EntityFrameworkCore;
using MyBuddy.Application.Tenancy;
using MyBuddy.Domain.Tenancy;
using MyBuddy.Infrastructure.Persistence;

namespace MyBuddy.Infrastructure.Tenancy;

public sealed class EfTenantMembershipReader(MyBuddyDbContext dbContext) : ITenantMembershipReader
{
    public Task<bool> HasActiveMembershipAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        dbContext.TenantMemberships
            .IgnoreQueryFilters()
            .AnyAsync(
                membership => membership.UserId == userId
                    && membership.TenantId == tenantId
                    && membership.Status == MembershipStatus.Active,
                cancellationToken);
}
