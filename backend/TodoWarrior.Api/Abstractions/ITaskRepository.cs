using System.Collections.Generic;
using TodoWarrior.Api.Models;

namespace TodoWarrior.Api.Abstractions
{
    public interface ITaskRepository
    {
        IQueryable<TaskItem> GetAll();
        IQueryable<TaskItem> GetActive();
        Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<TaskItem?> GetByGuidAsync(Guid guid, CancellationToken cancellationToken = default);
        Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task<TaskItem?> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid guid, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}