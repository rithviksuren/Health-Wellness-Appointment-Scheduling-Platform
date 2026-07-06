namespace NonProfitFund.Application.Contracts;

public sealed record UserDto(Guid Id, string Email, string DisplayName, string[] Roles, bool IsActive);
public sealed record SyncUserRequest(string ExternalIdentityId, string Email, string DisplayName, string? PhoneNumber);
public sealed record UpdateRolesRequest(string[] Roles);
public sealed record UpdateUserStatusRequest(bool IsActive);

public sealed record DonorProfileDto(Guid Id, string Email, string DisplayName, string? PhoneNumber, string? City, string? Country, bool EmailOptIn, bool SmsOptIn);
public sealed record UpdateDonorProfileRequest(string? DisplayName, string? PhoneNumber, string? AddressLine1, string? AddressLine2, string? City, string? State, string? PostalCode, string? Country, bool EmailOptIn, bool SmsOptIn);

public sealed record CampaignDto(Guid Id, string Name, string Slug, string Summary, decimal GoalAmount, decimal RaisedAmount, string Status, DateOnly StartsOn, DateOnly? EndsOn, string? HeroImageUrl);
public sealed record UpsertCampaignRequest(string Name, string Slug, string Summary, string? Story, decimal GoalAmount, DateOnly StartsOn, DateOnly? EndsOn, string? HeroImageUrl);

public sealed record ProjectDto(Guid Id, string Name, string Code, string Description, decimal FundingGoal, decimal AllocatedAmount, bool IsActive);
public sealed record UpsertProjectRequest(string Name, string Code, string Description, decimal FundingGoal, bool IsActive);

public sealed record DonationDto(Guid Id, Guid DonorId, Guid? CampaignId, decimal Amount, string Currency, string Status, DateTimeOffset CreatedAt);
public sealed record CreateDonationRequest(Guid? CampaignId, decimal Amount, string Currency, string? Dedication, bool GenerateReceipt);
public sealed record CreateRecurringDonationRequest(Guid? CampaignId, decimal Amount, string Currency, string Frequency, DateOnly NextRunOn);

public sealed record CreatePaymentIntentRequest(Guid DonationId, decimal Amount, string Currency);
public sealed record PaymentIntentResponse(string Provider, string ClientSecret, string ProviderReference);

public sealed record ReceiptDto(Guid Id, Guid DonationId, string ReceiptNumber, string Status, string? BlobUrl);
public sealed record NotificationDto(Guid Id, string Channel, string Status, string Subject, DateTimeOffset CreatedAt);
public sealed record ReportExportDto(Guid Id, string ReportType, string Status, string? BlobUrl);
public sealed record DonationSummaryDto(decimal TotalRaised, int DonationCount, decimal RecurringMonthlyValue, decimal AverageDonation);
public sealed record DashboardMetricDto(string Label, decimal Value, string Format);
public sealed record DashboardDto(DashboardMetricDto[] Metrics, CampaignDto[] Campaigns, DonationDto[] RecentDonations);

