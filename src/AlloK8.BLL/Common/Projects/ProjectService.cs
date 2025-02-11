using System;
using System.Collections.Generic;
using System.Linq;
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

    public DAL.Models.Project CreateProject(ProjectIM projectIM)
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

        var user = this.userService.GetUserProfileById(projectIM.CreatorId);
        user.Projects.Add(project);

        this.context.Update(user);
        this.context.SaveChanges();

        return project;
    }

    public DAL.Models.Project GetProjectById(int id)
    {
        var project = this.context.Projects.Find(id);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        return project;
    }

    public List<DAL.Models.Project> GetAllProjects()
    {
        return this.context.Projects.ToList();
    }

    public List<DAL.Models.Project> GetProjectsByUserId(Guid? userId)
    {
        var user = this.userService.GetUserProfileByGuid(userId);

        return this.context.Projects
            .Where(p => p.Users.Contains(user))
            .ToList();
    }

    public DAL.Models.Project UpdateTask(ProjectUM projectUM, int id)
    {
        var project = this.GetProjectById(id);

        project.Name = projectUM.Title ?? project.Name;
        project.Description = projectUM.Description ?? project.Description;

        this.context.Projects.Update(project);
        this.context.SaveChanges();

        return project;
    }

    public void DeleteProjectById(int id)
    {
        var project = this.GetProjectById(id);

        this.context.Projects.Remove(project);
        this.context.SaveChanges();
    }
}