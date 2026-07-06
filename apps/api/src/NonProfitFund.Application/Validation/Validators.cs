using FluentValidation;
using NonProfitFund.Application.Contracts;
using NonProfitFund.Domain.Enums;

namespace NonProfitFund.Application.Validation;

public sealed class SyncUserRequestValidator : AbstractValidator<SyncUserRequest>
{
    public SyncUserRequestValidator()
    {
        RuleFor(x => x.ExternalIdentityId).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(160);
    }
}

public sealed class UpdateRolesRequestValidator : AbstractValidator<UpdateRolesRequest>
{
    public UpdateRolesRequestValidator()
    {
        RuleFor(x => x.Roles).NotEmpty();
        RuleForEach(x => x.Roles).Must(role => Roles.All.Contains(role)).WithMessage("Unsupported role.");
    }
}

public sealed class UpsertCampaignRequestValidator : AbstractValidator<UpsertCampaignRequest>
{
    public UpsertCampaignRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(180);
        RuleFor(x => x.Slug).NotEmpty().Matches("^[a-z0-9-]+$").MaximumLength(120);
        RuleFor(x => x.Summary).NotEmpty().MaximumLength(500);
        RuleFor(x => x.GoalAmount).GreaterThan(0);
    }
}

public sealed class UpsertProjectRequestValidator : AbstractValidator<UpsertProjectRequest>
{
    public UpsertProjectRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(180);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.FundingGoal).GreaterThan(0);
    }
}

public sealed class CreateDonationRequestValidator : AbstractValidator<CreateDonationRequest>
{
    public CreateDonationRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

public sealed class CreateRecurringDonationRequestValidator : AbstractValidator<CreateRecurringDonationRequest>
{
    public CreateRecurringDonationRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Frequency).Must(x => x is "Monthly" or "Quarterly" or "Annual");
    }
}

