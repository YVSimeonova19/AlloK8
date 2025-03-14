using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.Common.Models.User;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.BLL.Common.Users;

internal class UserService : IUserService
{
    private readonly EntityContext context;

    public UserService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<UserProfile> CreateUserProfileAsync(Guid userId)
    {
        var userProflie = new UserProfile
        {
            ApplicationUserId = userId,
        };

        this.context.UserProfiles.Add(userProflie);
        await this.context.SaveChangesAsync();

        return userProflie;
    }

    public async Task<UserProfile> GetUserProfileByIdAsync(int id)
    {
        var user = this.context.UserProfiles.Find(id);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return user;
    }

    public async Task<UserProfile> GetUserProfileByGuidAsync(Guid? userGuid)
    {
        if (userGuid == null)
        {
            throw new ArgumentNullException(nameof(userGuid), "User GUID is null");
        }

        var userProfile = this.context.UserProfiles
            .FirstOrDefault(up => up.ApplicationUserId == userGuid);

        if (userProfile == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return userProfile;
    }

    public async Task<List<UserProfile>> GetAllUserProfilesAsync()
    {
        var users = this.context.UserProfiles.ToList();
        return users;
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
}