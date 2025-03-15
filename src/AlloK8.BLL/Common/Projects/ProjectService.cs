using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.Common.Models.Project;
using AlloK8.Common.Models.User;
using AlloK8.DAL;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.BLL.Common.Projects;

internal class ProjectService : IProjectService
{
    private readonly EntityContext context;
    private readonly IUserService userService;

    public ProjectService(
        EntityContext context,
        IUserService userService)
    {
        this.context = context;
        this.userService = userService;
    }

    public async Task<DAL.Models.Project> CreateProjectAsync(ProjectIM projectIM)
    {
        var project = new DAL.Models.Project
        {
            Name = projectIM.Name,
            Description = projectIM.Description,
            CreatedByUserId = projectIM.CreatorId,
            CreatedOn = projectIM.CreatedOn,
            UpdatedByUserId = projectIM.CreatorId,
            UpdatedOn = projectIM.CreatedOn,
        };

        var user = await this.userService.GetUserProfileByIdAsync(projectIM.CreatorId);
        user.Projects.Add(project);

        this.context.Update(user);
        await this.context.SaveChangesAsync();

        return project;
    }

    public async Task<DAL.Models.Project> GetProjectByIdAsync(int id)
    {
        var project = await this.context.Projects
            .Where(p => p.Id == id)
            .Include(p => p.Users)
            .ThenInclude(u => u.ApplicationUser)
            .FirstOrDefaultAsync();

        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        return project;
    }

    public async Task<List<DAL.Models.Project>> GetAllProjectsAsync()
    {
        return await this.context.Projects.ToListAsync();
    }

    public async Task<List<DAL.Models.Project>> GetProjectsByUserIdAsync(Guid? userId)
    {
        var user = await this.userService.GetUserProfileByGuidAsync(userId);

        return await this.context.Projects
            .Where(p => p.Users.Contains(user))
            .ToListAsync();
    }

    public async Task<DAL.Models.Project> UpdateProjectAsync(ProjectUM projectUM, int id)
    {
        var project = await this.GetProjectByIdAsync(id);

        project.Name = projectUM.Title ?? project.Name;
        project.Description = projectUM.Description ?? project.Description;

        this.context.Projects.Update(project);
        await this.context.SaveChangesAsync();

        return project;
    }

    public async Task DeleteProjectByIdAsync(int id)
    {
        var project = await this.GetProjectByIdAsync(id);

        this.context.Projects.Remove(project);
        await this.context.SaveChangesAsync();
    }

    public async Task AddUserToProjectAsync(int projectId, int userId)
    {
        var project = await this.GetProjectByIdAsync(projectId);
        var user = await this.userService.GetUserProfileByIdAsync(userId);

        if (project.Users.Any(u => u.Id == user.Id))
        {
            throw new Exception("User is already a member of this project.");
        }

        project.Users.Add(user);
        this.context.Update(project);
        await this.context.SaveChangesAsync();
    }

    public async Task RemoveUserFromProjectAsync(int projectId, int userId)
    {
        var project = await this.GetProjectByIdAsync(projectId);
        var user = await this.userService.GetUserProfileByIdAsync(userId);

        if (project.Users.Contains(user))
        {
            project.Users.Remove(user);
            this.context.Update(project);
            await this.context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("The user is not part of the project.");
        }
    }

    public async Task<List<UserVM>> GetAllUsersByProjectIdAsync(int projectId)
    {
        var project = await this.GetProjectByIdAsync(projectId);
        var userProfiles = project.Users.ToList();

        var userVMs = new List<UserVM>();

        foreach (var user in userProfiles)
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