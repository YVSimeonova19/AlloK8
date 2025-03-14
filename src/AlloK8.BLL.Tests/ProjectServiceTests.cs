using System;
using System.Collections.Generic;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Users;
using AlloK8.Common.Models.Project;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using FluentAssertions;
using Moq;
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
    
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project 1", Users = new List<UserProfile> { user } },
                new Project { Id = 2, Name = "Project 2", Users = new List<UserProfile> { user } }
            };

            await dbContext.UserProfiles.AddAsync(user);
            await dbContext.Projects.AddRangeAsync(projects);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProjectByIdAsync_ExistingId_ShouldReturnProject()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await projectService.GetProjectByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Project 1");
        }

        [Fact]
        public async Task GetProjectByIdAsync_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => projectService.GetProjectByIdAsync(999));
        }

        [Fact]
        public async Task GetAllProjectsAsync_ShouldReturnAllProjects()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await projectService.GetAllProjectsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task UpdateTaskAsync_ValidInput_ShouldReturnUpdatedProject()
        {
            // Arrange
            await SeedTestDataAsync();
            var projectUM = new ProjectUM { Title = "Updated Title", Description = "Updated Description" };

            // Act
            var result = await projectService.UpdateProjectAsync(projectUM, 1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(projectUM.Title);
            result.Description.Should().Be(projectUM.Description);

            var updatedProject = await dbContext.Projects.FindAsync(1);
            updatedProject.Name.Should().Be(projectUM.Title);
            updatedProject.Description.Should().Be(projectUM.Description);
        }

        [Fact]
        public async Task DeleteProjectByIdAsync_ValidId_ShouldDeleteProject()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            await projectService.DeleteProjectByIdAsync(1);

            // Assert
            var deletedProject = await dbContext.Projects.FindAsync(1);
            deletedProject.Should().BeNull();
        }

        [Fact]
        public async Task CreateProjectAsync_ValidInput_ShouldCreateAndReturnProject()
        {
            // Arrange
            var projectIM = new ProjectIM
            {
                Name = "New Project",
                Description = "Project Description",
                CreatorId = 1,
                CreatedOn = DateTime.UtcNow,
            };

            await SeedTestDataAsync();

            var user = this.dbContext.UserProfiles.Find(1);
            mockUserService.Setup(s => s.GetUserProfileByIdAsync(1))
                .ReturnsAsync(user);
            
            // Act
            var result = await projectService.CreateProjectAsync(projectIM);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(projectIM.Name);
            result.Description.Should().Be(projectIM.Description);
            dbContext.Projects.Should().Contain(result);
        }

        [Fact]
        public async Task GetProjectsByUserIdAsync_ValidUser_ShouldReturnProjects()
        {
            // Arrange
            await SeedTestDataAsync();

            var user = await dbContext.UserProfiles.FindAsync(1);
            mockUserService.Setup(s => s.GetUserProfileByGuidAsync(user.ApplicationUserId))
                .ReturnsAsync(user);

            // Act
            var result = await projectService.GetProjectsByUserIdAsync(user.ApplicationUserId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}