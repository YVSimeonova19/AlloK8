using System;
using AlloK8.DAL;
using AlloK8.DAL.Models;

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
}