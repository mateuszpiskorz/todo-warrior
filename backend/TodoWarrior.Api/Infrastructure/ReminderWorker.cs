using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;
using TodoWarrior.Api.Abstractions;
using TodoWarrior.Api.Hubs;
using TodoWarrior.Api.Data;


namespace TodoWarrior.Api.Infrastructure
{
    public class ReminderWorkerOptions
    {
        public bool Enabled { get; set; } = true;
        public int Horizon { get; set; } = 5;
        public int Interval { get; set; } = 30;
    }

    public class ReminderWorker : BackgroundService
    {
        private readonly ILogger<ReminderWorker> _logger;
        private readonly IReminderService _rs;
        private readonly IServiceScopeFactory _ssf;
        private readonly ReminderWorkerOptions _opts;
        private readonly IHubContext<RemindersHub> _hub;


        public ReminderWorker(
            ILogger<ReminderWorker> logger,
            IReminderService reminderService,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<ReminderWorkerOptions> options,
            IHubContext<RemindersHub> hub)
        {
            _logger = logger;
            _rs = reminderService;
            _ssf = serviceScopeFactory;
            _opts = options.Value;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {

            if (!_opts.Enabled)
            {
                _logger.LogInformation("Reminder Worker is disabled.");
                return;
            }

            _logger.LogInformation("Reminder Worker started at: {time}", DateTimeOffset.Now);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _ssf.CreateScope())
                    {
                        var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
                        var dueTasks = _rs.DueSoon(
                            await taskRepository.GetActiveAsync(),
                            TimeSpan.FromMinutes(_opts.Horizon));

                        foreach (var t in dueTasks)
                        {
                            await _hub.Clients.All.SendAsync("Reminder", new
                            {
                                t.Guid,
                                t.Title,
                                t.DueDate
                            }, ct);
                            _logger.LogInformation("Sent reminder for task {task} due at {due}", t.Title, t.DueDate);
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(_opts.Interval), ct);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogInformation("Reminder Worker is stopping: {message}", ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Reminder Worker error: {message}", ex.Message);
                }
            }
        }
    }
}