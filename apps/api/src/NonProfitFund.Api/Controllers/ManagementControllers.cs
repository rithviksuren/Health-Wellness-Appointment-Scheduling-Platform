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
[Route("api/users")]
[Authorize(Policy = Policies.AdminOnly)]
public sealed class UsersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<UserDto>> Get(CancellationToken ct) =>
        await dbContext.Users.Include(x => x.Roles)
            .Select(x => new UserDto(x.Id, x.Email, x.DisplayName, x.Roles.Select(r => r.Role).ToArray(), x.IsActive))
            .ToListAsync(ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken ct)
    {
        var user = await dbContext.Users.Include(x => x.Roles).SingleOrDefaultAsync(x => x.Id == id, ct);
        return user is null ? NotFound() : new UserDto(user.Id, user.Email, user.DisplayName, user.Roles.Select(r => r.Role).ToArray(), user.IsActive);
    }

    [HttpPatch("{id:guid}/roles")]
    public async Task<IActionResult> UpdateRoles(Guid id, UpdateRolesRequest request, CancellationToken ct)
    {
        var user = await dbContext.Users.Include(x => x.Roles).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (user is null) return NotFound();
        user.Roles.Clear();
        foreach (var role in request.Roles.Distinct()) user.Roles.Add(new UserRole { UserId = id, Role = role });
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateUserStatusRequest request, CancellationToken ct)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (user is null) return NotFound();
        user.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/donors")]
[Authorize(Policy = Policies.Donor)]
public sealed class DonorsController(AppDbContext dbContext, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<DonorProfileDto>> Me(CancellationToken ct)
    {
        var user = await ResolveUser(ct);
        return user is null ? NotFound() : ToDto(user);
    }

    [HttpPut("me")]
    public async Task<ActionResult<DonorProfileDto>> UpdateMe(UpdateDonorProfileRequest request, CancellationToken ct)
    {
        var user = await ResolveUser(ct);
        if (user is null) return NotFound();
        user.DisplayName = request.DisplayName ?? user.DisplayName;
        user.PhoneNumber = request.PhoneNumber;
        user.DonorProfile ??= new DonorProfile { UserId = user.Id };
        user.DonorProfile.AddressLine1 = request.AddressLine1;
        user.DonorProfile.AddressLine2 = request.AddressLine2;
        user.DonorProfile.City = request.City;
        user.DonorProfile.State = request.State;
        user.DonorProfile.PostalCode = request.PostalCode;
        user.DonorProfile.Country = request.Country;
        user.DonorProfile.EmailOptIn = request.EmailOptIn;
        user.DonorProfile.SmsOptIn = request.SmsOptIn;
        await dbContext.SaveChangesAsync(ct);
        return ToDto(user);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Treasury)]
    public async Task<ActionResult<DonorProfileDto>> GetById(Guid id, CancellationToken ct)
    {
        var user = await dbContext.Users.Include(x => x.DonorProfile).SingleOrDefaultAsync(x => x.Id == id, ct);
        return user is null ? NotFound() : ToDto(user);
    }

    private Task<User?> ResolveUser(CancellationToken ct) =>
        dbContext.Users.Include(x => x.DonorProfile).SingleOrDefaultAsync(x => x.ExternalIdentityId == currentUser.ExternalIdentityId, ct);

    private static DonorProfileDto ToDto(User user) =>
        new(user.Id, user.Email, user.DisplayName, user.PhoneNumber, user.DonorProfile?.City, user.DonorProfile?.Country, user.DonorProfile?.EmailOptIn ?? true, user.DonorProfile?.SmsOptIn ?? false);
}

