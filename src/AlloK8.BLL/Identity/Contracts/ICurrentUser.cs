using System;

namespace AlloK8.BLL.Identity.Contracts;

public interface ICurrentUser
{
    Guid? UserId { get; }
    bool Exists { get; }
}