namespace TodoWarrior.Api.Abstractions
{
    public interface IReminderClock
    {
        DateTimeOffset UtcNow { get; }
    }

    public class SystemClock : IReminderClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}