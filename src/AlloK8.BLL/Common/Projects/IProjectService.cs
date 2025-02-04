using System.Collections.Generic;
using AlloK8.Common.Models.Project;
using Microsoft.CodeAnalysis;

namespace AlloK8.BLL.Common.Projects;

public interface IProjectService
{
    DAL.Models.Project CreateProject(ProjectIM projectIM);

    DAL.Models.Project GetProjectById(int id);
    List<DAL.Models.Project> GetAllProjects();

    DAL.Models.Project UpdateTask(ProjectUM projectUM, int id);

    void DeleteProjectById(int id);
}