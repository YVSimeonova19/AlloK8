using System;
using System.Collections.Generic;
using AlloK8.BLL.Common.Labels;
using AlloK8.BLL.Common.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using FluentAssertions;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Tests;

public class TaskServiceTests
{
    private readonly EntityContext dbContext;
    private readonly UserService userService;
    private readonly TaskService taskService;
    private readonly LabelService labelService;

    public TaskServiceTests()
    {
        dbContext = TestHelpers.CreateDbContext();
        taskService = new TaskService(dbContext, userService, labelService);
    }

    private async Task SeedTestDataAsync()
    {
        var user1 = new UserProfile { Id = 1 };
        var user2 = new UserProfile { Id = 2 };
        
        var project1 = new Project { Id = 1, Name = "Project 1", Description = "First Project", CreatedByUserId = user1.Id };
        var project2 = new Project { Id = 2, Name = "Project 2", Description = "Second Project", CreatedByUserId = user2.Id };
        
        var column1 = new Column { Id = 1, Name = "todo" };
        var column2 = new Column { Id = 2, Name = "doing" };
        var column3 = new Column { Id = 3, Name = "done" };
        
        var task1 = new DAL.Models.Task
        {
            Id = 1,
            Title = "Task 1",
            Description = "First task description",
            ColumnId = column1.Id,
            ProjectId = project1.Id,
            Position = 1,
            StartDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(2),
            CreatedByUserId = user1.Id,
            CreatedOn = DateTime.Now,
            UpdatedByUserId = user1.Id,
            UpdatedOn = DateTime.Now
        };
        var task2 = new DAL.Models.Task
        {
            Id = 2,
            Title = "Task 2",
            Description = "Second task description",
            ColumnId = column1.Id,
            ProjectId = project1.Id,
            Position = 2,
            StartDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(3),
            CreatedByUserId = user1.Id,
            CreatedOn = DateTime.Now,
            UpdatedByUserId = user1.Id,
            UpdatedOn = DateTime.Now
        };
        var task3 = new DAL.Models.Task
        {
            Id = 3,
            Title = "Task 3",
            Description = "Task in progress",
            ColumnId = column2.Id,
            ProjectId = project2.Id,
            Position = 1,
            StartDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(4),
            CreatedByUserId = user2.Id,
            CreatedOn = DateTime.Now,
            UpdatedByUserId = user2.Id,
            UpdatedOn = DateTime.Now
        };
        var task4 = new DAL.Models.Task
        {
            Id = 4,
            Title = "Task 4",
            Description = "Completed task",
            ColumnId = column3.Id,
            ProjectId = project2.Id,
            Position = 1,
            StartDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(5),
            CreatedByUserId = user2.Id,
            CreatedOn = DateTime.Now,
            UpdatedByUserId = user2.Id,
            UpdatedOn = DateTime.Now
        };

        await dbContext.UserProfiles.AddRangeAsync(user1, user2);
        await dbContext.Projects.AddRangeAsync(project1, project2);
        await dbContext.Columns.AddRangeAsync(column1, column2, column3);
        await dbContext.Tasks.AddRangeAsync(task1, task2, task3, task4);

        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetTaskById_ExistingId_ShouldReturnTask()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var result = await taskService.GetTaskByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Task 1");
    }

    [Fact]
    public async Task GetTaskById_NonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => taskService.GetTaskByIdAsync(999));
    }

    [Fact]
    public async Task GetAllTasksByProjectId_ShouldReturnTasksForSpecificProject()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var result = await taskService.GetAllTasksByProjectIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task CreateTask_ValidInput_ShouldReturnNewTask()
    {
        // Arrange
        await SeedTestDataAsync();
        var taskIM = new TaskIM
        {
            Title = "New Task",
            Description = "New task description",
            ColumnId = 1,
            ProjectId = 1,
            CreatorId = 1,
            CreatedOn = DateTime.Now
        };

        // Act
        var result = await taskService.CreateTaskAsync(taskIM);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Description.Should().Be("New task description");
        result.ColumnId.Should().Be(1);
        result.ProjectId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateTask_ValidInput_ShouldReturnUpdatedTask()
    {
        // Arrange
        await SeedTestDataAsync();
        var taskUM = new TaskUM { Title = "Updated Title", Description = "Updated Description", Position = 3 };

        // Act
        var result = await taskService.UpdateTaskAsync(taskUM, 1);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(taskUM.Title);
        result.Description.Should().Be(taskUM.Description);
        result.Position.Should().Be(taskUM.Position);

        var updatedTask = await dbContext.Tasks.FindAsync(1);
        updatedTask.Title.Should().Be(taskUM.Title);
        updatedTask.Description.Should().Be(taskUM.Description);
        updatedTask.Position.Should().Be(taskUM.Position);
    }

    [Fact]
    public async Task MoveTask_ValidInput_ShouldMoveTaskAndUpdatePosition()
    {
        // Arrange
        await SeedTestDataAsync();
        var taskUM = new TaskUM { ColumnId = 2, Position = 1 };

        // Act
        var result = await taskService.MoveTaskAsync(taskUM, 1);

        // Assert
        result.Should().NotBeNull();
        result.ColumnId.Should().Be(2);
        result.Position.Should().Be(1);

        var movedTask = await dbContext.Tasks.FindAsync(1);
        movedTask.ColumnId.Should().Be(2);
        movedTask.Position.Should().Be(1);
    }

    [Fact]
    public async Task DeleteTaskById_ValidId_ShouldDeleteTask()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        await taskService.DeleteTaskByIdAsync(1);

        // Assert
        var deletedTask = await dbContext.Tasks.FindAsync(1);
        deletedTask.Should().BeNull();
    }
}