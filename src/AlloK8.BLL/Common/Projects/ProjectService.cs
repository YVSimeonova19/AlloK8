using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.Common.Models.Project;
using AlloK8.DAL;
using Microsoft.CodeAnalysis;

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
        this.context.SaveChanges();

        return project;
    }

    public async Task<DAL.Models.Project> GetProjectByIdAsync(int id)
    {
        var project = this.context.Projects.Find(id);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        return project;
    }

    public async Task<List<DAL.Models.Project>> GetAllProjectsAsync()
    {
        return this.context.Projects.ToList();
    }

    public async Task<List<DAL.Models.Project>> GetProjectsByUserIdAsync(Guid? userId)
    {
        var user = await this.userService.GetUserProfileByGuidAsync(userId);

        return this.context.Projects
            .Where(p => p.Users.Contains(user))
            .ToList();
    }

    public async Task<DAL.Models.Project> UpdateTaskAsync(ProjectUM projectUM, int id)
    {
        var project = await this.GetProjectByIdAsync(id);

        project.Name = projectUM.Title ?? project.Name;
        project.Description = projectUM.Description ?? project.Description;

        this.context.Projects.Update(project);
        this.context.SaveChanges();

        return project;
    }

    public async Task DeleteProjectByIdAsync(int id)
    {
        var project = await this.GetProjectByIdAsync(id);

        this.context.Projects.Remove(project);
        this.context.SaveChanges();
    }
}