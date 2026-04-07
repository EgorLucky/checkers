using Implementations.RepositoriesEF;
using Implementations.RepositoriesMongoDB;
using Microsoft.EntityFrameworkCore;
using MigrationTool;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var config = builder.Configuration;
var postgresConnectionString = config.GetValue<string>("checkerGameConnectionString");
var mongoDbConnectionString = config.GetValue<string>("checkersMongoConnectionString");
var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoDbConnectionString);


builder.Services.AddSingleton(mongoClientSettings)
    .AddScoped<IMongoClient, MongoClient>()
    .AddTransient<GameBoardStateMongoDBContext>()
    .AddDbContext<GameDbContext>(options =>
        options.UseNpgsql(
            postgresConnectionString,
            a => a.MigrationsAssembly(typeof(GameDbContext).Assembly.FullName)));

var host = builder.Build();
host.Run();