using System;
using System.Threading.Tasks;
using AlloK8.DAL.Models;

namespace AlloK8.BLL.Common.Users;

public interface IUserService
{
    Task<UserProfile> CreateUserProfile(Guid userId);

    Task<UserProfile> GetUserProfileById(int id);

    Task<UserProfile> GetUserProfileByGuid(Guid? userGuid);
}