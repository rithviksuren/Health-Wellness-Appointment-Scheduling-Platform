using Microsoft.EntityFrameworkCore;
using NonProfitFund.Application.Common;
using NonProfitFund.Domain.Common;
using NonProfitFund.Domain.Entities;

namespace NonProfitFund.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<DonorProfile> DonorProfiles => Set<DonorProfile>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<RecurringDonationPlan> RecurringDonationPlans => Set<RecurringDonationPlan>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FundAllocation> FundAllocations => Set<FundAllocation>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ReportExport> ReportExports => Set<ReportExport>();
    public DbSet<SocialCampaignLink> SocialCampaignLinks => Set<SocialCampaignLink>();

    IQueryable<User> IAppDbContext.Users => Users;
    IQueryable<Campaign> IAppDbContext.Campaigns => Campaigns;
    IQueryable<Project> IAppDbContext.Projects => Projects;
    IQueryable<Donation> IAppDbContext.Donations => Donations;
    IQueryable<RecurringDonationPlan> IAppDbContext.RecurringDonationPlans => RecurringDonationPlans;
    IQueryable<Receipt> IAppDbContext.Receipts => Receipts;
    IQueryable<Notification> IAppDbContext.Notifications => Notifications;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(x => x.Email).IsUnique();
            b.HasIndex(x => x.ExternalIdentityId).IsUnique();
            b.Property(x => x.Email).HasMaxLength(256);
            b.HasOne(x => x.DonorProfile).WithOne(x => x.User).HasForeignKey<DonorProfile>(x => x.UserId);
        });

        modelBuilder.Entity<UserRole>(b =>
        {
            b.HasIndex(x => new { x.UserId, x.Role }).IsUnique();
            b.Property(x => x.Role).HasMaxLength(64);
        });

        modelBuilder.Entity<Campaign>(b =>
        {
            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => new { x.Status, x.StartsOn, x.EndsOn });
            b.Property(x => x.GoalAmount).HasPrecision(18, 2);
            b.Property(x => x.RaisedAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Project>(b =>
        {
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.FundingGoal).HasPrecision(18, 2);
            b.Property(x => x.AllocatedAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Donation>(b =>
        {
            b.HasIndex(x => new { x.DonorId, x.CreatedAt });
            b.HasIndex(x => new { x.CampaignId, x.CreatedAt });
            b.HasIndex(x => x.Status);
            b.Property(x => x.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Payment>(b =>
        {
            b.HasIndex(x => x.ProviderReference);
            b.HasIndex(x => x.Status);
            b.Property(x => x.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<FundAllocation>(b =>
        {
            b.HasIndex(x => new { x.DonationId, x.ProjectId }).IsUnique();
            b.Property(x => x.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Receipt>().HasIndex(x => x.ReceiptNumber).IsUnique();
        modelBuilder.Entity<AuditLog>().HasIndex(x => new { x.ActorUserId, x.EntityType, x.CreatedAt });
    }
}

