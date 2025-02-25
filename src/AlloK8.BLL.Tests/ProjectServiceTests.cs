using System;
using System.Collections.Generic;
using System.Linq;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Users;
using AlloK8.Common.Models.Project;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using FluentAssertions;
using Moq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Tests
{
    public class ProjectServiceTests
    {
        private readonly EntityContext dbContext;
        private readonly Mock<IUserService> mockUserService;
        private readonly ProjectService projectService;

        public ProjectServiceTests()
        {
            dbContext = TestHelpers.CreateDbContext();
            mockUserService = new Mock<IUserService>();
            projectService = new ProjectService(dbContext, mockUserService.Object);
        }

        private async Task SeedTestDataAsync()
        {
            var user = new UserProfile { Id = 1 };
            var projects = new List<DAL.Models.Project>
            {
                new DAL.Models.Project { Id = 1, Name = "Project 1", Users = new List<UserProfile> { user } },
                new DAL.Models.Project { Id = 2, Name = "Project 2", Users = new List<UserProfile> { user } }
            };

            await dbContext.UserProfiles.AddAsync(user);
            await dbContext.Projects.AddRangeAsync(projects);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProjectById_ExistingId_ShouldReturnProject()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await projectService.GetProjectById(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Project 1");
        }

        [Fact]
        public async Task GetProjectById_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => projectService.GetProjectById(999));
        }

        [Fact]
        public async Task GetAllProjects_ShouldReturnAllProjects()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await projectService.GetAllProjects();

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task UpdateTask_ValidInput_ShouldReturnUpdatedProject()
        {
            // Arrange
            await SeedTestDataAsync();
            var projectUM = new ProjectUM { Title = "Updated Title", Description = "Updated Description" };

            // Act
            var result = await projectService.UpdateTask(projectUM, 1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(projectUM.Title);
            result.Description.Should().Be(projectUM.Description);

            var updatedProject = await dbContext.Projects.FindAsync(1);
            updatedProject.Name.Should().Be(projectUM.Title);
            updatedProject.Description.Should().Be(projectUM.Description);
        }

        [Fact]
        public async Task DeleteProjectById_ValidId_ShouldDeleteProject()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            await projectService.DeleteProjectById(1);

            // Assert
            var deletedProject = await dbContext.Projects.FindAsync(1);
            deletedProject.Should().BeNull();
        }
    }
}