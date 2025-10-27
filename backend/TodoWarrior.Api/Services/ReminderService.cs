using TodoWarrior.Api.Models;
using TodoWarrior.Api.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace TodoWarrior.Api.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderClock _clock;

        public ReminderService(IReminderClock clock)
        {
            _clock = clock;
        }

        public IEnumerable<TaskItem> DueSoon(IEnumerable<TaskItem> tasks, TimeSpan dueIn)
        {
            var now = _clock.UtcNow;
            var dueBy = now.Add(dueIn);

            return tasks.Where(t => t.ReminderAt != null &&
                                    t.ReminderAt > now &&
                                    t.ReminderAt <= dueBy);
        }
    }
}