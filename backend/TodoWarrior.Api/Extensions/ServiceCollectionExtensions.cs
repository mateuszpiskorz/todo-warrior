using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TodoWarrior.Api.Abstractions;
using TodoWarrior.Api.Data;
using TodoWarrior.Api.Contracts;
using TodoWarrior.Api.Repositories;
using TodoWarrior.Api.Services;
using FluentValidation;
using TodoWarrior.Api.Infrastructure;

namespace TodoWarrior.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            
            // Configure EF Core (SQLite database)
            var dbPath = configuration.GetValue<string>("SQLITE_PATH")
                ?? Environment.GetEnvironmentVariable("SQLITE_PATH")
                ?? Path.Combine(AppContext.BaseDirectory, "data", "app.db");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            
            // Add health checks
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

            // SignalR, CORS
            services.AddSignalR();
            services.AddCors(o => o.AddDefaultPolicy(p =>
                p.WithOrigins(configuration["FrontendOrigin"] ?? "http://localhost:5173")
                 .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            // Service registrations in DI container
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddSingleton<IReminderService, ReminderService>();
            services.AddSingleton<IReminderClock, SystemClock>();

            //Validators
            services.AddScoped<IValidator<CreateTaskItemDto>, CreateTaskItemDtoValidator>();
            services.AddScoped<IValidator<UpdateTaskItemDto>, UpdateTaskItemDtoValidator>();

            // Hosted service for reminders
            services.Configure<ReminderWorkerOptions>(configuration.GetSection("ReminderWorker"));
            if(configuration.GetValue<bool>("ReminderWorker:Enabled"))
            {
                services.AddHostedService<ReminderWorker>();
            }

            return services;
        } 
    }
}