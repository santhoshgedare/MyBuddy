using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuddy.Application.Tenancy;
using MyBuddy.Domain.Tenancy;
using MyBuddy.Infrastructure.Identity;

namespace MyBuddy.Infrastructure.Persistence;

public sealed class MyBuddyDbContext(
    DbContextOptions<MyBuddyDbContext> options,
    ITenantContext tenantContext)
    : IdentityDbContext<MyBuddyUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantDomain> TenantDomains => Set<TenantDomain>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();
    public DbSet<TenantRole> TenantRoles => Set<TenantRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<TenantRolePermission> TenantRolePermissions => Set<TenantRolePermission>();
    public DbSet<TenantUserRole> TenantUserRoles => Set<TenantUserRole>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<TenantModuleSetting> TenantModuleSettings => Set<TenantModuleSetting>();
    public DbSet<TenantIdentityProvider> TenantIdentityProviders => Set<TenantIdentityProvider>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tenant>(entity =>
        {
            entity.HasKey(tenant => tenant.Id);
            entity.HasIndex(tenant => tenant.Slug).IsUnique();
            entity.Property(tenant => tenant.Name).HasMaxLength(200).IsRequired();
            entity.Property(tenant => tenant.Slug).HasMaxLength(100).IsRequired();
        });

        ConfigureTenantOwned<TenantDomain>(builder, entity =>
        {
            entity.HasIndex(domain => new { domain.TenantId, domain.Hostname }).IsUnique();
            entity.Property(domain => domain.Hostname).HasMaxLength(253).IsRequired();
        });

        ConfigureTenantOwned<TenantMembership>(builder, entity =>
        {
            entity.HasIndex(membership => new { membership.TenantId, membership.UserId }).IsUnique();
            entity.HasOne<MyBuddyUser>()
                .WithMany()
                .HasForeignKey(membership => membership.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureTenantOwned<TenantRole>(builder, entity =>
        {
            entity.HasIndex(role => new { role.TenantId, role.Name }).IsUnique();
            entity.Property(role => role.Name).HasMaxLength(100).IsRequired();
        });

        builder.Entity<Permission>(entity =>
        {
            entity.HasKey(permission => permission.Id);
            entity.HasIndex(permission => permission.Code).IsUnique();
            entity.Property(permission => permission.Code).HasMaxLength(200).IsRequired();
        });

        ConfigureTenantOwned<TenantRolePermission>(builder, entity =>
        {
            entity.HasIndex(link => new { link.TenantId, link.TenantRoleId, link.PermissionId }).IsUnique();
            entity.HasOne<TenantRole>()
                .WithMany()
                .HasForeignKey(link => new { link.TenantId, link.TenantRoleId })
                .HasPrincipalKey(role => new { role.TenantId, role.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(link => link.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureTenantOwned<TenantUserRole>(builder, entity =>
        {
            entity.HasIndex(link => new { link.TenantId, link.TenantMembershipId, link.TenantRoleId }).IsUnique();
            entity.HasOne<TenantMembership>()
                .WithMany()
                .HasForeignKey(link => new { link.TenantId, link.TenantMembershipId })
                .HasPrincipalKey(membership => new { membership.TenantId, membership.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<TenantRole>()
                .WithMany()
                .HasForeignKey(link => new { link.TenantId, link.TenantRoleId })
                .HasPrincipalKey(role => new { role.TenantId, role.Id })
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureTenantOwned<TenantSubscription>(builder, entity =>
        {
            entity.HasIndex(subscription => new { subscription.TenantId, subscription.StartsAt });
            entity.Property(subscription => subscription.PlanCode).HasMaxLength(100).IsRequired();
        });

        ConfigureTenantOwned<TenantModuleSetting>(builder, entity =>
        {
            entity.HasIndex(setting => new { setting.TenantId, setting.ModuleCode }).IsUnique();
            entity.Property(setting => setting.ModuleCode).HasMaxLength(100).IsRequired();
        });

        ConfigureTenantOwned<TenantIdentityProvider>(builder, entity =>
        {
            entity.HasIndex(provider => new { provider.TenantId, provider.Authority, provider.ClientId }).IsUnique();
            entity.Property(provider => provider.Authority).HasMaxLength(2048).IsRequired();
            entity.Property(provider => provider.ClientId).HasMaxLength(200).IsRequired();
        });
    }

    private void ConfigureTenantOwned<TEntity>(
        ModelBuilder builder,
        Action<EntityTypeBuilder<TEntity>> configure)
        where TEntity : class, ITenantOwned
    {
        var entity = builder.Entity<TEntity>();
        entity.HasKey(value => value.Id);
        entity.HasAlternateKey(value => new { value.TenantId, value.Id });
        entity.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(value => value.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        entity.HasQueryFilter(value =>
            tenantContext.TenantId != null && value.TenantId == tenantContext.TenantId);
        configure(entity);
    }
}
