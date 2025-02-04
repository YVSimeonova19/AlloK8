using System.Collections.Generic;
using System.Linq;
using AlloK8.Common.Models.Project;
using AlloK8.DAL;
using Microsoft.CodeAnalysis;

namespace AlloK8.BLL.Common.Projects;

internal class ProjectService : IProjectService
{
    private readonly EntityContext context;

    public ProjectService(EntityContext context)
    {
        this.context = context;
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

        // TODO: How to add connection to ProjectUserProfile table

        this.context.Projects.Add(project);
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