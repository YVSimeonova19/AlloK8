using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlloK8.Common.Models.Project;

namespace AlloK8.BLL.Common.Projects;

public interface IProjectService
{
    Task<DAL.Models.Project> CreateProject(ProjectIM projectIM);

    Task<DAL.Models.Project> GetProjectById(int id);
    Task<List<DAL.Models.Project>> GetAllProjects();
    Task<List<DAL.Models.Project>> GetProjectsByUserId(Guid? userId);

    Task<DAL.Models.Project> UpdateTask(ProjectUM projectUM, int id);

    Task DeleteProjectById(int id);
}