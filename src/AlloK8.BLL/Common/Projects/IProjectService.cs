using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlloK8.Common.Models.Project;

namespace AlloK8.BLL.Common.Projects;

public interface IProjectService
{
    Task<DAL.Models.Project> CreateProjectAsync(ProjectIM projectIM);

    Task<DAL.Models.Project> GetProjectByIdAsync(int id);
    Task<List<DAL.Models.Project>> GetAllProjectsAsync();
    Task<List<DAL.Models.Project>> GetProjectsByUserIdAsync(Guid? userId);

    Task<DAL.Models.Project> UpdateTaskAsync(ProjectUM projectUM, int id);

    Task DeleteProjectByIdAsync(int id);
}