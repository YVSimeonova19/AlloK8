using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.Common.Models.User;
using AlloK8.DAL;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.BLL.Common.Search;

internal class SearchService : ISearchService
{
    private readonly EntityContext context;

    public SearchService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<List<UserVM>> SearchUsersByEmailAsync(string email)
    {
        var userVMs = new List<UserVM>();

        if (string.IsNullOrWhiteSpace(email))
        {
            return userVMs;
        }

        var users = await this.context.ApplicationUsers
            .Where(u => EF.Functions
                .Like(u.Email!.ToLower(), $"%{email.ToLower()}%"))
            .Include(u => u.UserProfile)
            .ToListAsync();

        foreach (var user in users)
        {
            userVMs.Add(new UserVM
            {
                Id = user.UserProfile!.Id,
                Email = user.Email,
            });
        }

        return userVMs;
    }

    public async Task<List<UserVM>> SearchTaskUsersByEmailAsync(string email, int projectId)
    {
        var userVMs = new List<UserVM>();

        if (string.IsNullOrWhiteSpace(email))
        {
            return userVMs;
        }

        var projectUsers = await this.context.Projects
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.Users)
            .Include(u => u.ApplicationUser)
            .Where(u => EF.Functions.Like(u.ApplicationUser!.Email!.ToLower(), $"%{email.ToLower()}%"))
            .ToListAsync();

        foreach (var user in projectUsers)
        {
            userVMs.Add(new UserVM
            {
                Id = user.Id,
                Email = user.ApplicationUser!.Email,
            });
        }

        return userVMs;
    }
}