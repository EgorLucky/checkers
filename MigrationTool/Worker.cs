using Implementations.RepositoriesEF;
using Implementations.RepositoriesMongoDB;
using Microsoft.EntityFrameworkCore;

namespace MigrationTool;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<BackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var mongoDbContext = scope.ServiceProvider.GetRequiredService<GameBoardStateMongoDBContext>();
            await mongoDbContext.ConfigureIndexesAsync();

            await using var dbContext = scope.ServiceProvider.GetService<GameDbContext>()!;
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await dbContext.Database.MigrateAsync(stoppingToken);
            });
        }
        catch (Exception ex)
        {
            logger.LogError("{}", ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }
}