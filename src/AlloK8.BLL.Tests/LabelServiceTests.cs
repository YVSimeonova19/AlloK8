using System;
using System.Collections.Generic;
using AlloK8.BLL.Common.Labels;
using AlloK8.Common.Models.Label;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using FluentAssertions;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Tests
{
    public class LabelServiceTests
    {
        private readonly EntityContext dbContext;
        private readonly LabelService labelService;

        public LabelServiceTests()
        {
            dbContext = TestHelpers.CreateDbContext();
            labelService = new LabelService(dbContext);
        }

        private async Task SeedTestDataAsync()
        {
            var project = new Project { Id = 1, Name = "Test Project" };
            var project2 = new Project { Id = 2, Name = "Test Project 2" };
            
            var labels = new List<Label>
            {
                new Label { Id = 1, Title = "Bug", Description = "Bug description", Color = "#FF0000", ProjectId = 1, Project = project },
                new Label { Id = 2, Title = "Feature", Description = "Feature description", Color = "#00FF00", ProjectId = 1, Project = project },
                new Label { Id = 3, Title = "Documentation", Description = "Documentation description", Color = "#0000FF", ProjectId = 2, Project = project2 }
            };

            var column = new Column { Id = 1, Name = "Todo" };
            
            var tasks = new List<DAL.Models.Task>
            {
                new DAL.Models.Task 
                { 
                    Id = 1, 
                    Title = "Task 1", 
                    Description = "Task 1 description", 
                    ColumnId = 1, 
                    Column = column,
                    ProjectId = 1,
                    Project = project,
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7)
                },
                new DAL.Models.Task
                {
                    Id = 2,
                    Title = "Task 2",
                    Description = "Task 2 description",
                    ColumnId = 1,
                    Column = column,
                    ProjectId = 1,
                    Project = project,
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7)
                }
            };

            // Add label-task relationships
            tasks[0].Labels.Add(labels[0]); // Task 1 has "Bug" label
            tasks[0].Labels.Add(labels[1]); // Task 1 has "Feature" label
            tasks[1].Labels.Add(labels[1]); // Task 2 has "Feature" label
            
            labels[0].Tasks.Add(tasks[0]); // "Bug" label is on Task 1
            labels[1].Tasks.Add(tasks[0]); // "Feature" label is on Task 1
            labels[1].Tasks.Add(tasks[1]); // "Feature" label is on Task 2

            await dbContext.Projects.AddRangeAsync(project, project2);
            await dbContext.Columns.AddAsync(column);
            await dbContext.Labels.AddRangeAsync(labels);
            await dbContext.Tasks.AddRangeAsync(tasks);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateLabelAsync_ValidInput_ShouldCreateAndReturnLabel()
        {
            // Arrange
            var project = new Project { Id = 1, Name = "Test Project" };
            await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();

            var labelIM = new LabelIM
            {
                Name = "Priority",
                Description = "High priority tasks",
                Color = "#FF00FF",
                ProjectId = 1
            };

            // Act
            var result = await labelService.CreateLabelAsync(labelIM);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(labelIM.Name);
            result.Description.Should().Be(labelIM.Description);
            result.Color.Should().Be(labelIM.Color);
            result.ProjectId.Should().Be(labelIM.ProjectId);

            var createdLabel = await dbContext.Labels.FindAsync(result.Id);
            createdLabel.Should().NotBeNull();
            createdLabel.Title.Should().Be(labelIM.Name);
        }

        [Fact]
        public async Task GetLabelByIdAsync_ExistingId_ShouldReturnLabel()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await labelService.GetLabelByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Bug");
            result.Color.Should().Be("#FF0000");
        }

        [Fact]
        public async Task GetLabelByIdAsync_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => labelService.GetLabelByIdAsync(999));
        }

        [Fact]
        public async Task GetLabelsByTaskIdAsync_ValidTaskId_ShouldReturnLabels()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await labelService.GetLabelsByTaskIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(l => l.Title == "Bug");
            result.Should().Contain(l => l.Title == "Feature");
        }

        [Fact]
        public async Task GetLabelsByTaskIdAsync_TaskWithNoLabels_ShouldReturnEmptyList()
        {
            // Arrange
            var project = new Project { Id = 1, Name = "Test Project" };
            var column = new Column { Id = 1, Name = "Todo" };
            var task = new DAL.Models.Task
            {
                Id = 3,
                Title = "Task with no labels",
                Description = "Description",
                ColumnId = 1,
                Column = column,
                ProjectId = 1,
                Project = project,
                StartDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            await dbContext.Projects.AddAsync(project);
            await dbContext.Columns.AddAsync(column);
            await dbContext.Tasks.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await labelService.GetLabelsByTaskIdAsync(3);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLabelsByProjectIdAsync_ValidProjectId_ShouldReturnLabels()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await labelService.GetLabelsByProjectIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(l => l.Title == "Bug");
            result.Should().Contain(l => l.Title == "Feature");
        }

        [Fact]
        public async Task GetLabelsByProjectIdAsync_ProjectWithNoLabels_ShouldReturnEmptyList()
        {
            // Arrange
            var project = new Project { Id = 3, Name = "Project with no labels" };
            await dbContext.Projects.AddAsync(project);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await labelService.GetLabelsByProjectIdAsync(3);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task EditLabelAsync_ValidInput_ShouldUpdateAndReturnLabel()
        {
            // Arrange
            await SeedTestDataAsync();
            
            var labelUM = new LabelUM
            {
                Title = "Updated Bug",
                Description = "Updated description",
                Color = "#990000"
            };

            // Act
            var result = await labelService.EditLabelAsync(labelUM, 1);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(labelUM.Title);
            result.Description.Should().Be(labelUM.Description);
            result.Color.Should().Be(labelUM.Color);

            var updatedLabel = await dbContext.Labels.FindAsync(1);
            updatedLabel.Should().NotBeNull();
            updatedLabel.Title.Should().Be(labelUM.Title);
            updatedLabel.Description.Should().Be(labelUM.Description);
            updatedLabel.Color.Should().Be(labelUM.Color);
        }

        [Fact]
        public async Task EditLabelAsync_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            await SeedTestDataAsync();
            var labelUM = new LabelUM 
            { 
                Title = "Updated Label", 
                Description = "Updated description", 
                Color = "#990000" 
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => labelService.EditLabelAsync(labelUM, 999));
        }

        [Fact]
        public async Task DeleteLabelAsync_ExistingId_ShouldDeleteLabel()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            await labelService.DeleteLabelAsync(1);

            // Assert
            var deletedLabel = await dbContext.Labels.FindAsync(1);
            deletedLabel.Should().BeNull();
        }

        [Fact]
        public async Task DeleteLabelAsync_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => labelService.DeleteLabelAsync(999));
        }
    }
}