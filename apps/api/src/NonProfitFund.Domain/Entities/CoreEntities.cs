using NonProfitFund.Domain.Common;
using NonProfitFund.Domain.Enums;

namespace NonProfitFund.Domain.Entities;

public sealed class User : Entity
{
    public string ExternalIdentityId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DonorProfile? DonorProfile { get; set; }
    public ICollection<UserRole> Roles { get; set; } = [];
}

public sealed class UserRole : Entity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string Role { get; set; } = string.Empty;
}

public sealed class DonorProfile : Entity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool EmailOptIn { get; set; } = true;
    public bool SmsOptIn { get; set; }
}

public sealed class Campaign : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Story { get; set; }
    public decimal GoalAmount { get; set; }
    public decimal RaisedAmount { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    public DateOnly StartsOn { get; set; }
    public DateOnly? EndsOn { get; set; }
    public string? HeroImageUrl { get; set; }
    public ICollection<Donation> Donations { get; set; } = [];
    public ICollection<SocialCampaignLink> SocialLinks { get; set; } = [];
}

public sealed class Project : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal FundingGoal { get; set; }
    public decimal AllocatedAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<FundAllocation> FundAllocations { get; set; } = [];
}

public sealed class Donation : Entity
{
    public Guid DonorId { get; set; }
    public User? Donor { get; set; }
    public Guid? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
    public Guid? RecurringDonationPlanId { get; set; }
    public RecurringDonationPlan? RecurringDonationPlan { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DonationStatus Status { get; set; } = DonationStatus.Pending;
    public string? Dedication { get; set; }
    public Payment? Payment { get; set; }
    public Receipt? Receipt { get; set; }
    public ICollection<FundAllocation> FundAllocations { get; set; } = [];
}

public sealed class RecurringDonationPlan : Entity
{
    public Guid DonorId { get; set; }
    public User? Donor { get; set; }
    public Guid? CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Frequency { get; set; } = "Monthly";
    public DateOnly NextRunOn { get; set; }
    public RecurringPlanStatus Status { get; set; } = RecurringPlanStatus.Active;
    public ICollection<Donation> Donations { get; set; } = [];
}

public sealed class Payment : Entity
{
    public Guid DonationId { get; set; }
    public Donation? Donation { get; set; }
    public string Provider { get; set; } = "Mock";
    public string ProviderReference { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Processing;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public sealed class FundAllocation : Entity
{
    public Guid DonationId { get; set; }
    public Donation? Donation { get; set; }
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public decimal Amount { get; set; }
}

public sealed class Receipt : Entity
{
    public Guid DonationId { get; set; }
    public Donation? Donation { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public ReceiptStatus Status { get; set; } = ReceiptStatus.Pending;
    public string? BlobUrl { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}

public sealed class Notification : Entity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset? SentAt { get; set; }
}

public sealed class AuditLog : Entity
{
    public Guid? ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? MetadataJson { get; set; }
    public string? IpAddress { get; set; }
}

public sealed class ReportExport : Entity
{
    public Guid RequestedByUserId { get; set; }
    public User? RequestedByUser { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Status { get; set; } = "Queued";
    public string? BlobUrl { get; set; }
}

public sealed class SocialCampaignLink : Entity
{
    public Guid CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
}

