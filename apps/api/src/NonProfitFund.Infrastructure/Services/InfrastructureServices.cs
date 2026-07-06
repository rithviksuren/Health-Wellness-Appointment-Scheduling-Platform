using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NonProfitFund.Application.Common;
using NonProfitFund.Application.Contracts;
using NonProfitFund.Domain.Entities;
using NonProfitFund.Domain.Enums;
using NonProfitFund.Infrastructure.Persistence;

namespace NonProfitFund.Infrastructure.Services;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Sql")));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IPaymentGateway, MockPaymentGateway>();
        services.AddScoped<IReceiptService, ReceiptService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}

internal sealed class MockPaymentGateway : IPaymentGateway
{
    public Task<PaymentIntentResponse> CreateIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken)
    {
        var reference = $"mock_{Guid.NewGuid():N}";
        return Task.FromResult(new PaymentIntentResponse("Mock", reference, reference));
    }

    public Task HandleWebhookAsync(string payload, string signature, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal sealed class ReceiptService(AppDbContext dbContext) : IReceiptService
{
    public async Task<ReceiptDto> GenerateAsync(Guid donationId, CancellationToken cancellationToken)
    {
        var receipt = new Receipt
        {
            DonationId = donationId,
            ReceiptNumber = $"R-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            Status = ReceiptStatus.Generated,
            BlobUrl = $"receipts/{donationId}.pdf"
        };
        dbContext.Receipts.Add(receipt);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReceiptDto(receipt.Id, receipt.DonationId, receipt.ReceiptNumber, receipt.Status.ToString(), receipt.BlobUrl);
    }

    public async Task ResendAsync(Guid receiptId, CancellationToken cancellationToken)
    {
        var receipt = await dbContext.Receipts.SingleAsync(x => x.Id == receiptId, cancellationToken);
        receipt.SentAt = DateTimeOffset.UtcNow;
        receipt.Status = ReceiptStatus.Sent;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class NotificationService(AppDbContext dbContext) : INotificationService
{
    public async Task QueueEmailAsync(Guid userId, string subject, string body, CancellationToken cancellationToken)
    {
        dbContext.Notifications.Add(new Notification { UserId = userId, Channel = NotificationChannel.Email, Subject = subject, Body = body });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task QueueSmsAsync(Guid userId, string body, CancellationToken cancellationToken)
    {
        dbContext.Notifications.Add(new Notification { UserId = userId, Channel = NotificationChannel.Sms, Subject = "SMS", Body = body });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class ReportService(AppDbContext dbContext, ICurrentUser currentUser) : IReportService
{
    public async Task<DonationSummaryDto> GetDonationSummaryAsync(CancellationToken cancellationToken)
    {
        var succeeded = dbContext.Donations.Where(x => x.Status == DonationStatus.Succeeded);
        var count = await succeeded.CountAsync(cancellationToken);
        var total = await succeeded.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;
        var monthly = await dbContext.RecurringDonationPlans
            .Where(x => x.Status == RecurringPlanStatus.Active)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;
        return new DonationSummaryDto(total, count, monthly, count == 0 ? 0 : total / count);
    }

    public async Task<ReportExportDto> QueueExportAsync(string reportType, CancellationToken cancellationToken)
    {
        var export = new ReportExport
        {
            RequestedByUserId = currentUser.UserId ?? Guid.Empty,
            ReportType = reportType,
            Status = "Queued"
        };
        dbContext.ReportExports.Add(export);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReportExportDto(export.Id, export.ReportType, export.Status, export.BlobUrl);
    }
}

