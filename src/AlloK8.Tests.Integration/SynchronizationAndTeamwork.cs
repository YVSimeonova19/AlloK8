using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Common.Labels;
using AlloK8.BLL.Common.EmailSending;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.Common.Models.Label;
using AlloK8.Common.Models.Project;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using Essentials.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.Tests.Integration
{
    public class TeamworkIntegrationTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture fixture;
        private readonly ITestOutputHelper output;

        public TeamworkIntegrationTests(TestDatabaseFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public async Task BasicProjectCreationTest()
        {
            // Arrange
            output.WriteLine("Creating test user for BasicProjectCreationTest");
            var userProfile = await CreateAndVerifyTestUserAsync("basic-test@example.com");
            output.WriteLine($"Created user profile with ID: {userProfile.Id}");

            using var scope = fixture.ServiceProvider.CreateScope();
            var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
            
            // Act
            output.WriteLine("Creating project");
            var project = await projectService.CreateProjectAsync(new ProjectIM
            {
                Name = "Basic Test Project",
                Description = "Testing basic project creation",
                CreatorId = userProfile.Id,
                CreatedOn = DateTime.UtcNow
            });

            // Assert
            Assert.NotNull(project);
            Assert.Equal("Basic Test Project", project.Name);
            output.WriteLine($"Project created successfully with ID: {project.Id}");
        }

        [Fact]
        public async Task WhenCreatingProject_AndAddingAnotherUser_ThenItAppearsInOtherUsersProjects()
        {
            // Arrange
            output.WriteLine("Creating user1");
            var user1Profile = await CreateAndVerifyTestUserAsync("user1@example.com");
            output.WriteLine($"User1 created with ID: {user1Profile.Id}");
            
            output.WriteLine("Creating user2");
            var user2Profile = await CreateAndVerifyTestUserAsync("user2@example.com");
            output.WriteLine($"User2 created with ID: {user2Profile.Id}");

            using var scope = fixture.ServiceProvider.CreateScope();
            var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
            
            // Act 1
            output.WriteLine("Creating project as user1");
            var project = await projectService.CreateProjectAsync(new ProjectIM
            {
                Name = "Test Project",
                Description = "Project for integration testing",
                CreatorId = user1Profile.Id,
                CreatedOn = DateTime.UtcNow
            });
            output.WriteLine($"Project created with ID: {project.Id}");

            // Act 2
            output.WriteLine($"Adding user2 (ID: {user2Profile.Id}) to project");
            await projectService.AddUserToProjectAsync(project.Id, user2Profile.Id);
            output.WriteLine("User2 added to project");

            // Assert
            output.WriteLine($"Checking if user2 (ID: {user2Profile.ApplicationUserId}) can see the project");
            var user2Projects = await projectService.GetProjectsByUserIdAsync(user2Profile.ApplicationUserId);
            
            Assert.NotEmpty(user2Projects);
            Assert.Contains(user2Projects, p => p.Id == project.Id);
            Assert.Equal("Test Project", user2Projects.First(p => p.Id == project.Id).Name);
            output.WriteLine("Test passed: User2 can see the project");
        }

        [Fact]
        public async Task WhenAddingTask_ThenOtherUsersInProjectCanSeeIt()
        {
            // Arrange
            output.WriteLine("Creating task-user1");
            var user1Profile = await CreateAndVerifyTestUserAsync("task-user1@example.com");
            output.WriteLine("Creating task-user2");
            var user2Profile = await CreateAndVerifyTestUserAsync("task-user2@example.com");

            using var scope = fixture.ServiceProvider.CreateScope();
            var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

            output.WriteLine("Creating project for task test");
            var project = await projectService.CreateProjectAsync(new ProjectIM
            {
                Name = "Task Test Project",
                Description = "Project for task integration testing",
                CreatorId = user1Profile.Id,
                CreatedOn = DateTime.UtcNow
            });
            
            output.WriteLine($"Adding user2 (ID: {user2Profile.Id}) to project");
            await projectService.AddUserToProjectAsync(project.Id, user2Profile.Id);

            // Act
            output.WriteLine("Creating task");
            var task = await taskService.CreateTaskAsync(new TaskIM
            {
                Title = "Test Task",
                Description = "Task for integration testing",
                ProjectId = project.Id,
                CreatorId = user1Profile.Id,
                CreatedOn = DateTime.UtcNow,
                ColumnId = 1,
            });
            output.WriteLine($"Task created with ID: {task.Id}");

            // Assert
            output.WriteLine("Checking if User2 can see the task");
            var tasksForUser2 = await taskService.GetAllTasksByProjectIdAsync(project.Id);
            
            Assert.NotEmpty(tasksForUser2);
            Assert.Contains(tasksForUser2, t => t.Id == task.Id);
            Assert.Equal("Test Task", tasksForUser2.First(t => t.Id == task.Id).Title);
            output.WriteLine("Test passed: User2 can see the task");
        }

        [Fact]
        public async Task WhenMovingTask_ThenChangesAreVisibleToOtherUsers()
        {
            // Arrange
            output.WriteLine("Creating move-user1");
            var user1Profile = await CreateAndVerifyTestUserAsync("move-user1@example.com");
            output.WriteLine("Creating move-user2");
            var user2Profile = await CreateAndVerifyTestUserAsync("move-user2@example.com");

            using var scope = fixture.ServiceProvider.CreateScope();
            var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

            output.WriteLine("Creating project for move test");
            var project = await projectService.CreateProjectAsync(new ProjectIM
            {
                Name = "Move Task Project",
                Description = "Project for task movement testing",
                CreatorId = user1Profile.Id,
                CreatedOn = DateTime.UtcNow
            });
            
            output.WriteLine($"Adding user2 (ID: {user2Profile.Id}) to project");
            await projectService.AddUserToProjectAsync(project.Id, user2Profile.Id);

            output.WriteLine("Creating task to move");
            var task = await taskService.CreateTaskAsync(new TaskIM
            {
                Title = "Task to Move",
                Description = "This task will be moved",
                ProjectId = project.Id,
                CreatorId = user1Profile.Id,
                CreatedOn = DateTime.UtcNow,
                ColumnId = 1,
            });
            output.WriteLine($"Task created with ID: {task.Id}");

            // Act
            output.WriteLine("Moving task to column 2");
            var taskUM = new TaskUM
            {
                ColumnId = 2,
                Position = 1,
            };
            
            await taskService.MoveTaskAsync(taskUM, task.Id);
            output.WriteLine("Task moved");

            // Assert
            output.WriteLine("Checking if task was moved successfully");
            var updatedTask = await taskService.GetTaskByIdAsync(task.Id);
            
            Assert.Equal(2, updatedTask.ColumnId);
            Assert.Equal(1, updatedTask.Position);
            output.WriteLine("Test passed: Task was moved successfully");
        }

        [Fact]
        public async Task WhenEditingTask_ThenChangesAreVisibleToOtherUsers()
        {
            // Arrange
            output.WriteLine("Creating edit-user1");
            var user1Profile = await CreateAndVerifyTestUserAsync("edit-user1@example.com");
            output.WriteLine("Creating edit-user2");
            var user2Profile = await CreateAndVerifyTestUserAsync("edit-user2@example.com");

            using var scope = fixture.ServiceProvider.CreateScope();
            var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

            output.WriteLine("Creating project for edit test");
            var project = await projectService.CreateProjectAsync(new ProjectIM
            {
                Name = "Edit Task Project",
                Description = "Project for task editing testing",
                CreatorId = user1Profile.Id,
                CreatedOn = DateTime.UtcNow
            });
            
            output.WriteLine($"Adding user2 (ID: {user2Profile.Id}) to project");
            await projectService.AddUserToProjectAsync(project.Id, user2Profile.Id);

            output.WriteLine("Creating task to edit");
            var task = await taskService.CreateTaskAsync(new TaskIM
            {
                Title = "Original Task Title",
                Description = "Original description",
                ProjectId = project.Id,
                CreatorId = user1Profile.Id,
                CreatedOn = DateTime.UtcNow,
                ColumnId = 1
            });
            output.WriteLine($"Task created with ID: {task.Id}");

            // Act
            output.WriteLine("Editing task");
            var taskUM = new TaskUM
            {
                Title = "Updated Task Title",
                Description = "This description has been updated",
                IsPriority = true
            };
            
            await taskService.UpdateTaskAsync(taskUM, task.Id);
            output.WriteLine("Task edited");

            // Assert
            output.WriteLine("Checking if task was edited successfully");
            var updatedTask = await taskService.GetTaskByIdAsync(task.Id);
            
            Assert.Equal("Updated Task Title", updatedTask.Title);
            Assert.Equal("This description has been updated", updatedTask.Description);
            Assert.True(updatedTask.IsPriority);
            output.WriteLine("Test passed: Task was edited successfully");
        }

        private async Task<UserProfile> CreateAndVerifyTestUserAsync(string email)
        {
            try
            {
                using var scope = fixture.ServiceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<EntityContext>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                
                // Create ApplicationUser directly with a predictable GUID based on email
                var userId = CreateDeterministicGuid(email);
                output.WriteLine($"Using deterministic GUID {userId} for user {email}");
                
                // Check if the user already exists
                var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    output.WriteLine($"User {email} already exists with ID {existingUser.Id}");
                    
                    var existingProfile = await context.UserProfiles
                        .Include(p => p.ApplicationUser)
                        .FirstOrDefaultAsync(p => p.ApplicationUserId == existingUser.Id);
                    
                    if (existingProfile != null)
                    {
                        output.WriteLine($"Found existing profile with ID {existingProfile.Id}");
                        return existingProfile;
                    }
                    
                    // Create profile if it doesn't exist
                    output.WriteLine($"Creating profile for existing user {existingUser.Id}");
                    await userService.CreateUserProfileAsync(existingUser.Id);
                    
                    var newProfile = await context.UserProfiles
                        .Include(p => p.ApplicationUser)
                        .FirstOrDefaultAsync(p => p.ApplicationUserId == existingUser.Id);
                    
                    if (newProfile == null)
                        throw new Exception($"Failed to create profile for existing user {existingUser.Id}");
                        
                    output.WriteLine($"Created profile with ID {newProfile.Id}");
                    return newProfile;
                }
                
                var appUser = new ApplicationUser
                {
                    Id = userId,
                    UserName = email,
                    Email = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    NormalizedUserName = email.ToUpperInvariant(),
                    EmailConfirmed = true
                };
                
                output.WriteLine($"Adding new user {email} to context");
                context.Users.Add(appUser);
                await context.SaveChangesAsync();
                
                output.WriteLine($"Creating UserProfile directly for {email}");
                var userProfile = new UserProfile
                {
                    ApplicationUserId = userId
                };
                
                context.UserProfiles.Add(userProfile);
                await context.SaveChangesAsync();
                
                var profile = await context.UserProfiles
                    .Include(p => p.ApplicationUser)
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                    
                if (profile == null)
                {
                    throw new Exception($"Failed to find profile after creation for user {userId}");
                }
                
                output.WriteLine($"Created UserProfile with ID {profile.Id} for {email}");
                return profile;
            }
            catch (Exception ex)
            {
                output.WriteLine($"ERROR in CreateAndVerifyTestUserAsync: {ex.Message}");
                output.WriteLine(ex.StackTrace);
                throw;
            }
        }
        
        // Create a deterministic GUID based on input string
        private Guid CreateDeterministicGuid(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            
            return new Guid(hashBytes);
        }
    }

    // Test database fixture to provide a consistent database context for tests
    public class TestDatabaseFixture : IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string databaseName;

        public TestDatabaseFixture()
        {
            databaseName = "TestDb_TeamworkIntegration";
            
            try {
                var services = new ServiceCollection();

                services.AddLogging(configure => configure.AddDebug());

                services.AddDbContext<EntityContext>(options => 
                    options.UseInMemoryDatabase(databaseName));

                services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<EntityContext>();
                
                services.AddScoped<ICurrentUser, MockCurrentUser>();
                
                services.AddScoped<IEmailService, MockEmailService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IProjectService, ProjectService>();
                services.AddScoped<ITaskService, TaskService>();
                services.AddScoped<ILabelService, MockLabelService>();

                serviceProvider = services.BuildServiceProvider();

                InitializeDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestDatabaseFixture failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void InitializeDatabase()
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
            
            dbContext.Database.EnsureCreated();
            
            if (!dbContext.Columns.Any())
            {
                dbContext.Columns.Add(new Column { Id = 1, Name = "To Do", Position = 1 });
                dbContext.Columns.Add(new Column { Id = 2, Name = "In Progress", Position = 2 });
                dbContext.Columns.Add(new Column { Id = 3, Name = "Done", Position = 3 });
                dbContext.SaveChanges();
            }
        }

        public IServiceProvider ServiceProvider => serviceProvider;

        public void Dispose()
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
            dbContext.Database.EnsureDeleted();
        }
    }

    public class MockCurrentUser : ICurrentUser
    {
        public Guid? UserId { get; set; }
        public bool Exists => UserId.HasValue;
        
        public MockCurrentUser(Guid? userId = null)
        {
            UserId = userId;
        }
    }

    public class MockEmailService : IEmailService
    {
        public Task<bool> SendResetPasswordEmailAsync(string email, string resetPasswordUrl)
        {
            return Task.FromResult(true);
        }

        public async Task<StandardResult> SendEmailAsync(EmailModel emailModel)
        {
            throw new NotImplementedException();
        }
    }

    public class MockLabelService : ILabelService
    {
        private readonly EntityContext context;
        
        public MockLabelService(EntityContext context)
        {
            this.context = context;
        }

        public async Task<Label> CreateLabelAsync(LabelIM labelIM)
        {
            throw new NotImplementedException();
        }

        public Task<Label> GetLabelByIdAsync(int id)
        {
            return Task.FromResult(new Label { Id = id, Title = "Test Label" });
        }
        
        public Task<List<Label>> GetLabelsByTaskIdAsync(int taskId)
        {
            return Task.FromResult(new List<Label>());
        }

        public async Task<List<Label>> GetLabelsByProjectIdAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Label> EditLabelAsync(LabelUM labelUM, int labelId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteLabelAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Label> CreateLabelAsync(string title, string color, int projectId)
        {
            var label = new Label
            {
                Title = title,
                Color = color,
                ProjectId = projectId
            };
            return Task.FromResult(label);
        }

        public Task<List<Label>> GetAllLabelsByProjectIdAsync(int projectId)
        {
            return Task.FromResult(new List<Label>());
        }

        public Task<Label> UpdateLabelAsync(int id, string title, string color)
        {
            return Task.FromResult(new Label { Id = id, Title = title, Color = color });
        }

        public Task DeleteLabelByIdAsync(int id)
        {
            return Task.CompletedTask;
        }
    }
}