using System.Security.Claims;
using NonProfitFund.Application.Common;

namespace NonProfitFund.Api.Security;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue("app_user_id");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? ExternalIdentityId => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? accessor.HttpContext?.User.FindFirstValue("sub");

    public string? Email => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
        ?? accessor.HttpContext?.User.FindFirstValue("emails");

    public bool IsInRole(string role) => accessor.HttpContext?.User.IsInRole(role) == true;
}

