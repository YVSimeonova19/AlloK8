using System;
using System.Threading.Tasks;
using AlloK8.DAL.Models;

namespace AlloK8.BLL.Common.Users;

public interface IUserService
{
    Task<UserProfile> CreateUserProfileAsync(Guid userId);

    Task<UserProfile> GetUserProfileByIdAsync(int id);

    Task<UserProfile> GetUserProfileByGuidAsync(Guid? userGuid);
}