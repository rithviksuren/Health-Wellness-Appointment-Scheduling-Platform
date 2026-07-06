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
[Route("api/campaigns")]
public sealed class CampaignsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<CampaignDto>> Get(CancellationToken ct) =>
        await dbContext.Campaigns.OrderByDescending(x => x.CreatedAt).Select(ToDto).ToListAsync(ct);

    [HttpGet("{slug}")]
    public async Task<ActionResult<CampaignDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var campaign = await dbContext.Campaigns.SingleOrDefaultAsync(x => x.Slug == slug, ct);
        return campaign is null ? NotFound() : ToDto.Compile()(campaign);
    }

    [Authorize(Policy = Policies.CampaignManagement)]
    [HttpPost]
    public async Task<ActionResult<CampaignDto>> Create(UpsertCampaignRequest request, CancellationToken ct)
    {
        var campaign = new Campaign
        {
            Name = request.Name,
            Slug = request.Slug,
            Summary = request.Summary,
            Story = request.Story,
            GoalAmount = request.GoalAmount,
            StartsOn = request.StartsOn,
            EndsOn = request.EndsOn,
            HeroImageUrl = request.HeroImageUrl
        };
        dbContext.Campaigns.Add(campaign);
        await dbContext.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetBySlug), new { slug = campaign.Slug }, ToDto.Compile()(campaign));
    }

    [Authorize(Policy = Policies.CampaignManagement)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertCampaignRequest request, CancellationToken ct)
    {
        var campaign = await dbContext.Campaigns.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (campaign is null) return NotFound();
        campaign.Name = request.Name;
        campaign.Slug = request.Slug;
        campaign.Summary = request.Summary;
        campaign.Story = request.Story;
        campaign.GoalAmount = request.GoalAmount;
        campaign.StartsOn = request.StartsOn;
        campaign.EndsOn = request.EndsOn;
        campaign.HeroImageUrl = request.HeroImageUrl;
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Policy = Policies.CampaignManagement)]
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var campaign = await dbContext.Campaigns.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (campaign is null) return NotFound();
        campaign.Status = CampaignStatus.Published;
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    private static readonly System.Linq.Expressions.Expression<Func<Campaign, CampaignDto>> ToDto =
        x => new CampaignDto(x.Id, x.Name, x.Slug, x.Summary, x.GoalAmount, x.RaisedAmount, x.Status.ToString(), x.StartsOn, x.EndsOn, x.HeroImageUrl);
}

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ProjectDto>> Get(CancellationToken ct) =>
        await dbContext.Projects.Select(x => new ProjectDto(x.Id, x.Name, x.Code, x.Description, x.FundingGoal, x.AllocatedAmount, x.IsActive)).ToListAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken ct)
    {
        var project = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == id, ct);
        return project is null ? NotFound() : new ProjectDto(project.Id, project.Name, project.Code, project.Description, project.FundingGoal, project.AllocatedAmount, project.IsActive);
    }

    [Authorize(Policy = Policies.CampaignManagement)]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(UpsertProjectRequest request, CancellationToken ct)
    {
        var project = new Project { Name = request.Name, Code = request.Code, Description = request.Description, FundingGoal = request.FundingGoal, IsActive = request.IsActive };
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, new ProjectDto(project.Id, project.Name, project.Code, project.Description, project.FundingGoal, project.AllocatedAmount, project.IsActive));
    }

    [Authorize(Policy = Policies.CampaignManagement)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpsertProjectRequest request, CancellationToken ct)
    {
        var project = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (project is null) return NotFound();
        project.Name = request.Name;
        project.Code = request.Code;
        project.Description = request.Description;
        project.FundingGoal = request.FundingGoal;
        project.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/donations")]
[Authorize(Policy = Policies.Donor)]
public sealed class DonationsController(AppDbContext dbContext, ICurrentUser currentUser, IReceiptService receiptService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<DonationDto>> Create(CreateDonationRequest request, CancellationToken ct)
    {
        var donor = await dbContext.Users.SingleOrDefaultAsync(x => x.ExternalIdentityId == currentUser.ExternalIdentityId, ct);
        if (donor is null) return Problem("Authenticated user must be synced before donating.", statusCode: StatusCodes.Status409Conflict);

        var donation = new Donation { DonorId = donor.Id, CampaignId = request.CampaignId, Amount = request.Amount, Currency = request.Currency, Dedication = request.Dedication, Status = DonationStatus.Succeeded };
        dbContext.Donations.Add(donation);

        if (request.CampaignId.HasValue)
        {
            var campaign = await dbContext.Campaigns.SingleOrDefaultAsync(x => x.Id == request.CampaignId, ct);
            if (campaign is not null) campaign.RaisedAmount += request.Amount;
        }

        await dbContext.SaveChangesAsync(ct);
        if (request.GenerateReceipt) await receiptService.GenerateAsync(donation.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = donation.Id }, ToDto(donation));
    }

    [HttpGet("me")]
    public async Task<IReadOnlyList<DonationDto>> Mine(CancellationToken ct)
    {
        var donor = await dbContext.Users.SingleOrDefaultAsync(x => x.ExternalIdentityId == currentUser.ExternalIdentityId, ct);
        if (donor is null) return [];
        return await dbContext.Donations.Where(x => x.DonorId == donor.Id).OrderByDescending(x => x.CreatedAt).Select(x => ToDto(x)).ToListAsync(ct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DonationDto>> GetById(Guid id, CancellationToken ct)
    {
        var donation = await dbContext.Donations.SingleOrDefaultAsync(x => x.Id == id, ct);
        return donation is null ? NotFound() : ToDto(donation);
    }

    private static DonationDto ToDto(Donation x) => new(x.Id, x.DonorId, x.CampaignId, x.Amount, x.Currency, x.Status.ToString(), x.CreatedAt);
}

