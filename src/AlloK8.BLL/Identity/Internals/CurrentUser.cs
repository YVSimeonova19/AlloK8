using System;
using System.Linq;
using System.Security.Claims;
using AlloK8.BLL.Identity.Contracts;
using Microsoft.AspNetCore.Http;

namespace AlloK8.BLL.Identity.Internals;

internal class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => this.GetUserId();
    public bool Exists => this.UserId.HasValue;

    private Guid? GetUserId()
    {
        var rawUserId = this.httpContextAccessor
            .HttpContext?
            .User?
            .Claims?
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
            .Value;
        if (string.IsNullOrWhiteSpace(rawUserId))
        {
            return null;
        }

        if (Guid.TryParse(rawUserId, out var userId))
        {
            return userId;
        }

        return null;
    }
}