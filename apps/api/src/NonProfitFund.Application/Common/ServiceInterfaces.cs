using NonProfitFund.Application.Contracts;
using NonProfitFund.Domain.Entities;

namespace NonProfitFund.Application.Common;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? ExternalIdentityId { get; }
    string? Email { get; }
    bool IsInRole(string role);
}

public interface IAppDbContext
{
    IQueryable<User> Users { get; }
    IQueryable<Campaign> Campaigns { get; }
    IQueryable<Project> Projects { get; }
    IQueryable<Donation> Donations { get; }
    IQueryable<RecurringDonationPlan> RecurringDonationPlans { get; }
    IQueryable<Receipt> Receipts { get; }
    IQueryable<Notification> Notifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IPaymentGateway
{
    Task<PaymentIntentResponse> CreateIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken);
    Task HandleWebhookAsync(string payload, string signature, CancellationToken cancellationToken);
}

public interface IReceiptService
{
    Task<ReceiptDto> GenerateAsync(Guid donationId, CancellationToken cancellationToken);
    Task ResendAsync(Guid receiptId, CancellationToken cancellationToken);
}

public interface INotificationService
{
    Task QueueEmailAsync(Guid userId, string subject, string body, CancellationToken cancellationToken);
    Task QueueSmsAsync(Guid userId, string body, CancellationToken cancellationToken);
}

public interface IReportService
{
    Task<DonationSummaryDto> GetDonationSummaryAsync(CancellationToken cancellationToken);
    Task<ReportExportDto> QueueExportAsync(string reportType, CancellationToken cancellationToken);
}

