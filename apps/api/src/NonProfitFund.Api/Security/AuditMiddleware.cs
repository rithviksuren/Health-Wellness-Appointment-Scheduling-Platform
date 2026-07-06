using NonProfitFund.Domain.Entities;
using NonProfitFund.Infrastructure.Persistence;

namespace NonProfitFund.Api.Security;

public sealed class AuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        await next(context);

        if (context.Request.Method is "POST" or "PUT" or "PATCH" or "DELETE")
        {
            dbContext.AuditLogs.Add(new AuditLog
            {
                ActorUserId = Guid.TryParse(context.User.FindFirst("app_user_id")?.Value, out var id) ? id : null,
                Action = $"{context.Request.Method} {context.Request.Path}",
                EntityType = "HttpRequest",
                IpAddress = context.Connection.RemoteIpAddress?.ToString()
            });
            await dbContext.SaveChangesAsync(context.RequestAborted);
        }
    }
}

