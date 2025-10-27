using FluentAssertions;
using Moq;
using TodoWarrior.Api.Services;
using TodoWarrior.Api.Abstractions;
using TodoWarrior.Api.Models;
using Xunit;

namespace TodoWarrior.Api.Tests.Services;

public class ReminderServiceTests
{
    private readonly Mock<IReminderClock> _mockClock;
    private readonly ReminderService _service;

    public ReminderServiceTests()
    {
        _mockClock = new Mock<IReminderClock>();
        _service = new ReminderService(_mockClock.Object);
    }

    [Fact]
    public void DueSoon_WithNoTasks_ShouldReturnEmpty()
    {
        _mockClock.Setup(c => c.UtcNow).Returns(DateTime.UtcNow);
        var tasks = new List<TaskItem>();

        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(5));

        Assert.Empty(result);
    }

    [Fact]
    public void DueSoon_WithTasksWithoutReminders_ShouldReturnEmpty()
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var tasks = new List<TaskItem>
        {
            new TaskItem { Guid = Guid.NewGuid(), Title = "Task 1", ReminderAt = null },
            new TaskItem { Guid = Guid.NewGuid(), Title = "Task 2", ReminderAt = null }
        };

        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(5));

        Assert.Empty(result);
    }

    [Fact]
    public void DueSoon_WithTaskDueInTimeWindow_ShouldReturnTask()
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var tasks = new List<TaskItem>
        {
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Due Soon", 
                ReminderAt = now.AddMinutes(3)
            }
        };

        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(5));

        Assert.Single(result);
        Assert.Equal("Due Soon", result.First().Title);
    }

    [Fact]
    public void DueSoon_WithTaskDueExactlyAtWindowEnd_ShouldReturnTask()
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var tasks = new List<TaskItem>
        {
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Due at Boundary", 
                ReminderAt = now.AddMinutes(5) // Exactly 5 minutes
            }
        };

        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(5));

        Assert.Single(result);
        Assert.Equal("Due at Boundary", result.First().Title);
    }

    [Fact]
    public void DueSoon_WithTaskDueBeyondWindow_ShouldNotReturnTask()
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var tasks = new List<TaskItem>
        {
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Too Far", 
                ReminderAt = now.AddMinutes(10)
            }
        };


        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(5));

        Assert.Empty(result);
    }

    [Fact]
    public void DueSoon_WithPastDueTask_ShouldNotReturnTask()
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var tasks = new List<TaskItem>
        {
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Overdue", 
                ReminderAt = now.AddMinutes(-5) // 5 minutes ago
            }
        };

        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(5));

        Assert.Empty(result);
    }

    [Fact]
    public void DueSoon_WithTaskDueExactlyNow_ShouldNotReturnTask()
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var tasks = new List<TaskItem>
        {
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Due Now", 
                ReminderAt = now
            }
        };


        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(5));

        Assert.Empty(result);
    }

    [Theory]
    [MemberData(nameof(GetMultipleTaskScenarios))]
    public void DueSoon_WithMultipleTasks_ShouldReturnOnlyTasksInWindow(
        DateTime now,
        List<TaskItem> tasks,
        TimeSpan window,
        int expectedCount)
    {
        _mockClock.Setup(c => c.UtcNow).Returns(now);

     
        var result = _service.DueSoon(tasks, window);

        Assert.Equal(expectedCount, result.Count());
    }

    public static IEnumerable<object[]> GetMultipleTaskScenarios()
    {
        var baseTime = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);

        yield return new object[]
        {
            baseTime,
            new List<TaskItem>
            {
                new TaskItem { Guid = Guid.NewGuid(), Title = "Task 1", ReminderAt = baseTime.AddMinutes(2) },
                new TaskItem { Guid = Guid.NewGuid(), Title = "Task 2", ReminderAt = baseTime.AddMinutes(4) },
                new TaskItem { Guid = Guid.NewGuid(), Title = "Task 3", ReminderAt = baseTime.AddMinutes(6) },
            },
            TimeSpan.FromMinutes(5),
            2 
        };

        yield return new object[]
        {
            baseTime,
            new List<TaskItem>
            {
                new TaskItem { Guid = Guid.NewGuid(), Title = "Past", ReminderAt = baseTime.AddMinutes(-1) },
                new TaskItem { Guid = Guid.NewGuid(), Title = "Soon 1", ReminderAt = baseTime.AddMinutes(1) },
                new TaskItem { Guid = Guid.NewGuid(), Title = "Soon 2", ReminderAt = baseTime.AddMinutes(3) },
                new TaskItem { Guid = Guid.NewGuid(), Title = "Later", ReminderAt = baseTime.AddMinutes(10) },
                new TaskItem { Guid = Guid.NewGuid(), Title = "No Reminder", ReminderAt = null },
            },
            TimeSpan.FromMinutes(5),
            2 // Only "Soon 1" and "Soon 2"
        };

        yield return new object[]
        {
            baseTime,
            new List<TaskItem>
            {
                new TaskItem { Guid = Guid.NewGuid(), Title = "Task 1", ReminderAt = baseTime.AddHours(1) },
                new TaskItem { Guid = Guid.NewGuid(), Title = "Task 2", ReminderAt = baseTime.AddHours(2) },
            },
            TimeSpan.FromMinutes(30),
            0 // No tasks within 30 minutes
        };
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(60)]
    public void DueSoon_WithDifferentTimeWindows_ShouldWorkCorrectly(int windowMinutes)
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var halfWindow = windowMinutes / 2.0;
        var tasks = new List<TaskItem>
        {
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Within Window", 
                ReminderAt = now.AddMinutes(halfWindow) 
            },
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Beyond Window", 
                ReminderAt = now.AddMinutes(windowMinutes + 1) 
            }
        };

        var result = _service.DueSoon(tasks, TimeSpan.FromMinutes(windowMinutes));

        Assert.Single(result);
        Assert.Equal("Within Window", result.First().Title);
    }

    [Fact]
    public void DueSoon_WithZeroTimeWindow_ShouldReturnEmpty()
    {
        var now = new DateTime(2025, 10, 26, 12, 0, 0, DateTimeKind.Utc);
        _mockClock.Setup(c => c.UtcNow).Returns(now);

        var tasks = new List<TaskItem>
        {
            new TaskItem 
            { 
                Guid = Guid.NewGuid(), 
                Title = "Task", 
                ReminderAt = now.AddSeconds(1) 
            }
        };

        var result = _service.DueSoon(tasks, TimeSpan.Zero);

        Assert.Empty(result);
    }
}