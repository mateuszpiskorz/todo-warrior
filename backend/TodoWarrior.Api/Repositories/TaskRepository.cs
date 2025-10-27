using Microsoft.EntityFrameworkCore;
using TodoWarrior.Api.Abstractions;
using TodoWarrior.Api.Data;
using TodoWarrior.Api.Models;

namespace TodoWarrior.Api.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TaskItem> GetAll()
        {
            return _dbContext.TaskItems;
        }

        public IQueryable<TaskItem> GetActive()
        {
            return _dbContext.TaskItems.Where(t => !t.IsDone && t.IsActive);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.TaskItems.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.TaskItems
                .Where(t => !t.IsDone && t.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<TaskItem?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Guid == guid, cancellationToken);
        }

        public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            var entityEntry = await _dbContext.TaskItems.AddAsync(task, cancellationToken);
            return entityEntry.Entity;
        }

        public async Task<TaskItem?> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            var existingTask = await GetByGuidAsync(task.Guid, cancellationToken);
            if (existingTask == null) return null;

            _dbContext.Entry(existingTask).CurrentValues.SetValues(task);
            return await Task.FromResult(existingTask);
        }

        public async Task<bool> DeleteAsync(Guid guid, CancellationToken cancellationToken = default)
        {
            var task = await GetByGuidAsync(guid, cancellationToken);
            if (task != null)
            {
                task.IsActive = false;
                _dbContext.TaskItems.Update(task);
                return true;
            }
            return false;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}