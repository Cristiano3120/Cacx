using CacxServer.UserDataDatabaseResources;

namespace CacxServer.Services;

public sealed class UnverifiedUserCleanupService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            UserDataDbContext db = scope.ServiceProvider.GetRequiredService<UserDataDbContext>();

            int deadlineInHours = 24;
            DateTime cutoff = DateTime.UtcNow.AddHours(-deadlineInHours);
            IQueryable<DbUser> oldUnverifiedUsers = db.Users
                .Where(x => !x.Verified && x.CreatedAt < cutoff);

            db.Users.RemoveRange(oldUnverifiedUsers);
            _ = await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
