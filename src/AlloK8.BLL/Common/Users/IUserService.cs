using System;
using AlloK8.DAL.Models;

namespace AlloK8.BLL.Common.Users;

public interface IUserService
{
    UserProfile CreateUserProfile(Guid userId);
}