using TodoWarrior.Api.Repositories;
using TodoWarrior.Api.Data;
using TodoWarrior.Api.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TodoWarrior.Api.Tests.Repositories
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly TaskRepository _repository;
        private readonly ApplicationDbContext _dbContext;
        public TaskRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _repository = new TaskRepository(_dbContext);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllTasksAsync_ReturnsAllTasks()
        {
            var task1 = new TaskItem { Title = "Task 1" };
            var task2 = new TaskItem { Title = "Task 2" };
            _dbContext.TaskItems.AddRange(task1, task2);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Title == "Task 1");
            Assert.Contains(result, t => t.Title == "Task 2");
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsTask()
        {
            var task1 = new TaskItem { Guid = Guid.NewGuid(), Title = "Task 1" };
            var task2 = new TaskItem { Guid = Guid.NewGuid(), Title = "Task 2" };

            _dbContext.TaskItems.AddRange(task1, task2);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByGuidAsync(task1.Guid);


            Assert.NotNull(result);
            Assert.Equal(task1.Guid, result.Guid);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddTaskToDatabase()
        {
            var task = new TaskItem
            {
                Title = "New Task",
                Description = "New Description",
                IsDone = false
            };

            var result = await _repository.AddAsync(task);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Guid);
            Assert.Equal("New Task", result.Title);

            var savedTask = await _dbContext.TaskItems.FindAsync(result.Guid);
            Assert.NotNull(savedTask);
            Assert.Equal("New Task", savedTask.Title);
        }

        [Fact]
        public async Task UpdateAsync_WhenTaskExists_ShouldUpdateTask()
        {
            var task = new TaskItem
            {
                Title = "Original",
                Description = "Original Desc",
                IsDone = false
            };
            
            await _dbContext.TaskItems.AddAsync(task);
            await _dbContext.SaveChangesAsync();

            task.Title = "Updated";
            task.Description = "Updated Desc";
            task.IsDone = true;

            var result = await _repository.UpdateAsync(task);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Title);
            Assert.Equal("Updated Desc", result.Description);
            Assert.True(result.IsDone);

            var updatedTask = await _dbContext.TaskItems.FindAsync(task.Guid);
            Assert.Equal("Updated", updatedTask!.Title);
        }

        [Fact]
        public async Task UpdateAsync_WhenTaskDoesNotExist_ShouldReturnNull()
        {
            var nonExistentTask = new TaskItem
            {
                Guid = Guid.NewGuid(),
                Title = "Non-existent"
            };

            var result = await _repository.UpdateAsync(nonExistentTask);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WhenTaskExists_ShouldReturnTrue()
        {
            var task = new TaskItem { Title = "To Delete" };
            await _dbContext.TaskItems.AddAsync(task);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.DeleteAsync(task.Guid);

            Assert.True(result);
            var deletedTask = await _dbContext.TaskItems.FindAsync(task.Guid);
            Assert.False(deletedTask!.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_WhenTaskDoesNotExist_ShouldReturnFalse()
        {
            var result = await _repository.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task GetCompletedTasksAsync_ShouldReturnOnlyCompletedTasks()
        {
            var tasks = new[]
            {
                new TaskItem { Title = "Completed 1", IsDone = true },
                new TaskItem { Title = "Not Completed", IsDone = false },
                new TaskItem { Title = "Completed 2", IsDone = true }
            };
            await _dbContext.TaskItems.AddRangeAsync(tasks);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetAllAsync();
            var completedTasks = result.Where(t => t.IsDone).ToList();

            Assert.Equal(2, completedTasks.Count);
            Assert.All(completedTasks, t => Assert.True(t.IsDone));
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
