using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonProfitFund.Application.Common;
using NonProfitFund.Application.Contracts;
using NonProfitFund.Domain.Entities;
using NonProfitFund.Domain.Enums;
using NonProfitFund.Infrastructure.Persistence;

namespace NonProfitFund.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AppDbContext dbContext, ICurrentUser currentUser) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> Me(CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.Include(x => x.Roles)
            .SingleOrDefaultAsync(x => x.ExternalIdentityId == currentUser.ExternalIdentityId, cancellationToken);
        return user is null
            ? NotFound()
            : new UserDto(user.Id, user.Email, user.DisplayName, user.Roles.Select(x => x.Role).ToArray(), user.IsActive);
    }

    [Authorize]
    [HttpPost("sync-user")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> SyncUser(SyncUserRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.Include(x => x.Roles)
            .SingleOrDefaultAsync(x => x.ExternalIdentityId == request.ExternalIdentityId, cancellationToken);

        if (user is null)
        {
            user = new User
            {
                ExternalIdentityId = request.ExternalIdentityId,
                Email = request.Email,
                DisplayName = request.DisplayName,
                PhoneNumber = request.PhoneNumber,
                Roles = [new UserRole { Role = Roles.Donor }]
            };
            dbContext.Users.Add(user);
        }
        else
        {
            user.Email = request.Email;
            user.DisplayName = request.DisplayName;
            user.PhoneNumber = request.PhoneNumber;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UserDto(user.Id, user.Email, user.DisplayName, user.Roles.Select(x => x.Role).ToArray(), user.IsActive);
    }
}

