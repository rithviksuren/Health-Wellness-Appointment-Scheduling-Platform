namespace NonProfitFund.Domain.Enums;

public static class Roles
{
    public const string Donor = "Donor";
    public const string Volunteer = "Volunteer";
    public const string Treasurer = "Treasurer";
    public const string CampaignManager = "Campaign Manager";
    public const string Admin = "Admin";

    public static readonly string[] All =
    [
        Donor,
        Volunteer,
        Treasurer,
        CampaignManager,
        Admin
    ];
}

