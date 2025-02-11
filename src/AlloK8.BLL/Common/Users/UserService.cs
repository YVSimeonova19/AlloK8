using System;
using System.Collections.Generic;
using System.Linq;
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

    public UserProfile CreateUserProfile(Guid userId)
    {
        var userProflie = new UserProfile
        {
            ApplicationUserId = userId,
        };

        this.context.UserProfiles.Add(userProflie);
        this.context.SaveChanges();

        return userProflie;
    }

    public UserProfile GetUserProfileById(int id)
    {
        var user = this.context.UserProfiles.Find(id);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return user;
    }

    public UserProfile GetUserProfileByGuid(Guid? userGuid)
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
}