using TodoWarrior.Api.Models;

namespace TodoWarrior.Api.Abstractions
{
    public interface IReminderService
    {
        IEnumerable<TaskItem> DueSoon(IEnumerable<TaskItem> tasks, TimeSpan dueIn);
    }
}