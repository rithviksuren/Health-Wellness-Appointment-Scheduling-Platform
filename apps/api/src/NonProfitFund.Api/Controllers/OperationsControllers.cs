using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonProfitFund.Api.Security;
using NonProfitFund.Application.Common;
using NonProfitFund.Application.Contracts;
using NonProfitFund.Domain.Entities;
using NonProfitFund.Domain.Enums;
using NonProfitFund.Infrastructure.Persistence;

namespace NonProfitFund.Api.Controllers;

[ApiController]
[Route("api/recurring-donations")]
[Authorize(Policy = Policies.Donor)]
public sealed class RecurringDonationsController(AppDbContext dbContext, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateRecurringDonationRequest request, CancellationToken ct)
    {
        var donor = await dbContext.Users.SingleOrDefaultAsync(x => x.ExternalIdentityId == currentUser.ExternalIdentityId, ct);
        if (donor is null) return Conflict();
        var plan = new RecurringDonationPlan { DonorId = donor.Id, CampaignId = request.CampaignId, Amount = request.Amount, Currency = request.Currency, Frequency = request.Frequency, NextRunOn = request.NextRunOn };
        dbContext.RecurringDonationPlans.Add(plan);
        await dbContext.SaveChangesAsync(ct);
        return Created($"/api/recurring-donations/{plan.Id}", new { plan.Id, plan.Status });
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var plan = await dbContext.RecurringDonationPlans.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (plan is null) return NotFound();
        plan.Status = RecurringPlanStatus.Cancelled;
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(IPaymentGateway paymentGateway) : ControllerBase
{
    [Authorize(Policy = Policies.Donor)]
    [HttpPost("intent")]
    public Task<PaymentIntentResponse> CreateIntent(CreatePaymentIntentRequest request, CancellationToken ct) =>
        paymentGateway.CreateIntentAsync(request, ct);

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        await paymentGateway.HandleWebhookAsync(await reader.ReadToEndAsync(ct), Request.Headers["x-signature"].ToString(), ct);
        return Ok();
    }
}

[ApiController]
[Route("api/receipts")]
[Authorize(Policy = Policies.Donor)]
public sealed class ReceiptsController(AppDbContext dbContext, IReceiptService receiptService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReceiptDto>> Get(Guid id, CancellationToken ct)
    {
        var receipt = await dbContext.Receipts.SingleOrDefaultAsync(x => x.Id == id, ct);
        return receipt is null ? NotFound() : new ReceiptDto(receipt.Id, receipt.DonationId, receipt.ReceiptNumber, receipt.Status.ToString(), receipt.BlobUrl);
    }

    [HttpPost("{id:guid}/resend")]
    public async Task<IActionResult> Resend(Guid id, CancellationToken ct)
    {
        await receiptService.ResendAsync(id, ct);
        return Accepted();
    }
}

[ApiController]
[Route("api/reports")]
[Authorize(Policy = Policies.Treasury)]
public sealed class ReportsController(IReportService reports) : ControllerBase
{
    [HttpGet("donation-summary")]
    public Task<DonationSummaryDto> Summary(CancellationToken ct) => reports.GetDonationSummaryAsync(ct);

    [HttpGet("monthly")]
    public ActionResult<object> Monthly() => Ok(new { periods = Array.Empty<object>() });

    [HttpGet("campaigns")]
    public ActionResult<object> Campaigns() => Ok(new { campaigns = Array.Empty<object>() });

    [HttpGet("donors")]
    public ActionResult<object> Donors() => Ok(new { donors = Array.Empty<object>() });

    [HttpGet("project-funding")]
    public ActionResult<object> ProjectFunding() => Ok(new { projects = Array.Empty<object>() });

    [HttpPost("export")]
    public Task<ReportExportDto> Export([FromQuery] string reportType, CancellationToken ct) => reports.QueueExportAsync(reportType, ct);
}

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(AppDbContext dbContext, INotificationService notifications, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<NotificationDto>> Get(CancellationToken ct)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.ExternalIdentityId == currentUser.ExternalIdentityId, ct);
        if (user is null) return [];
        return await dbContext.Notifications.Where(x => x.UserId == user.Id).OrderByDescending(x => x.CreatedAt)
            .Select(x => new NotificationDto(x.Id, x.Channel.ToString(), x.Status.ToString(), x.Subject, x.CreatedAt)).ToListAsync(ct);
    }

    [HttpPost("test")]
    public async Task<IActionResult> Test(CancellationToken ct)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.ExternalIdentityId == currentUser.ExternalIdentityId, ct);
        if (user is null) return NotFound();
        await notifications.QueueEmailAsync(user.Id, "Test notification", "Your notification pipeline is configured.", ct);
        return Accepted();
    }
}

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController(AppDbContext dbContext, IReportService reports) : ControllerBase
{
    [HttpGet("donor")]
    public Task<DashboardDto> Donor(CancellationToken ct) => BuildDashboard(ct);

    [HttpGet("admin")]
    [Authorize(Policy = Policies.AdminOnly)]
    public Task<DashboardDto> Admin(CancellationToken ct) => BuildDashboard(ct);

    [HttpGet("treasurer")]
    [Authorize(Policy = Policies.Treasury)]
    public Task<DashboardDto> Treasurer(CancellationToken ct) => BuildDashboard(ct);

    [HttpGet("campaign-manager")]
    [Authorize(Policy = Policies.CampaignManagement)]
    public Task<DashboardDto> CampaignManager(CancellationToken ct) => BuildDashboard(ct);

    private async Task<DashboardDto> BuildDashboard(CancellationToken ct)
    {
        var summary = await reports.GetDonationSummaryAsync(ct);
        var campaigns = await dbContext.Campaigns.OrderByDescending(x => x.RaisedAmount).Take(5)
            .Select(x => new CampaignDto(x.Id, x.Name, x.Slug, x.Summary, x.GoalAmount, x.RaisedAmount, x.Status.ToString(), x.StartsOn, x.EndsOn, x.HeroImageUrl)).ToArrayAsync(ct);
        var donations = await dbContext.Donations.OrderByDescending(x => x.CreatedAt).Take(10)
            .Select(x => new DonationDto(x.Id, x.DonorId, x.CampaignId, x.Amount, x.Currency, x.Status.ToString(), x.CreatedAt)).ToArrayAsync(ct);
        return new DashboardDto(
            [
                new("Total Raised", summary.TotalRaised, "currency"),
                new("Donations", summary.DonationCount, "number"),
                new("Recurring MRR", summary.RecurringMonthlyValue, "currency"),
                new("Average Gift", summary.AverageDonation, "currency")
            ],
            campaigns,
            donations);
    }
}

