using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Implementations.RepositoriesEF;
using Implementations.RepositoriesMongoDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Tests;

public class AspireCustomWebApplicationFactory
{
    private DistributedApplication? _app;
    private IDistributedApplicationTestingBuilder? _appHost;
    private HttpClient? _client;
    private ResourceNotificationService _resourceNotificationService;

    public AspireCustomWebApplicationFactory()
    {
        _appHost = DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireAppHost>().GetAwaiter().GetResult();
        
        _appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        _appHost.Services.AddLogging();
        
        _app = _appHost.BuildAsync().GetAwaiter().GetResult();
        
        _resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        _app.StartAsync().GetAwaiter().GetResult();
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _resourceNotificationService.WaitForResourceAsync("postgres", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(20));
        _resourceNotificationService.WaitForResourceAsync("mongodb", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(20));
        _resourceNotificationService.WaitForResourceAsync("rabbitmq", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(20));
        _resourceNotificationService.WaitForResourceAsync("bot-worker", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(20));
        _resourceNotificationService.WaitForResourceAsync("rest-api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(20));
        
        stopwatch.Stop();
        Console.WriteLine($"Waited {stopwatch.ElapsedMilliseconds} ms for required resources to start");
     }

    public HttpClient CreateClient()
    {
        _appHost.Resources.First(r => r.Name == "rest-api");
        _client = _app.CreateHttpClient("rest-api");
        return _client!;
    }

    public async Task<GameDbContext> GetDbContext()
    {
        var logger = _appHost.ExecutionContext.ServiceProvider.GetRequiredService<ILogger<GameDbContext>>();
        
        var pgResource = (PostgresServerResource)_appHost.Resources.First(r => r.Name == "postgres");
        var databaseName = pgResource.Databases.First().Value;
        var databaseResource = (PostgresDatabaseResource)_appHost.Resources.First(r => r.Name == databaseName);
        var connectionString = await (new ConnectionStringReference(databaseResource, false)).Resource.GetConnectionStringAsync();
        
        return new GameDbContext(
            new DbContextOptionsBuilder<GameDbContext>()
                .UseNpgsql(connectionString).LogTo(log => logger.LogDebug(log)).Options);
    }

    public async Task<GameBoardStateMongoDBContext> GetMongoDbContext()
    {
        var mongoResource = (MongoDBServerResource)_appHost.Resources.First(r => r.Name == "mongodb");
        var connectionString = await (new ConnectionStringReference(mongoResource, false)).Resource.GetConnectionStringAsync();
        
        return new GameBoardStateMongoDBContext(new MongoClient(connectionString));
    }
}