var builder = DistributedApplication.CreateBuilder(args);

var postgresServer = builder.AddPostgres("postgres").AddDatabase("checker-game-postgres");
var mongoDbServer = builder.AddMongoDB("mongodb").AddDatabase("checker-game-mongo");
var rabbitMqServer = builder.AddRabbitMQ("rabbitmq");

var migrationTool = builder.AddProject<Projects.MigrationTool>("migration-tool")
    .WithReference(postgresServer)
    .WithReference(mongoDbServer)
    .WithEnvironment(context =>
    {
        context.EnvironmentVariables["checkerGameConnectionString"] =
            new ConnectionStringReference(postgresServer.Resource, false);
        context.EnvironmentVariables["checkersMongoConnectionString"] = 
            new ConnectionStringReference(mongoDbServer.Resource, false);
    })
    .WaitFor(postgresServer)
    .WaitFor(mongoDbServer);

var api = builder.AddProject<Projects.RestApi>("rest-api")
    .WithReference(postgresServer)
    .WithReference(mongoDbServer)
    .WithReference(rabbitMqServer)
    .WithReference(migrationTool)
    .WithEnvironment(context =>
    {
        context.EnvironmentVariables["checkerGameConnectionString"] =
            new ConnectionStringReference(postgresServer.Resource, false);
        context.EnvironmentVariables["checkersMongoConnectionString"] = 
            new ConnectionStringReference(mongoDbServer.Resource, false);
        context.EnvironmentVariables["checkerGameRabbitMqConnectionString"] = 
            new ConnectionStringReference(rabbitMqServer.Resource, false);
    })
    .WaitFor(postgresServer)
    .WaitFor(mongoDbServer)
    .WaitFor(rabbitMqServer)
    .WaitForCompletion(migrationTool);

var botWorker = builder.AddProject<Projects.BotWorkerService>("bot-worker")
    .WithReference(api)
    .WithReference(postgresServer)
    .WithReference(rabbitMqServer).WithEnvironment(context =>
    {
        context.EnvironmentVariables["checkerGameConnectionString"] =
            new ConnectionStringReference(postgresServer.Resource, false);
        context.EnvironmentVariables["checkerGameRabbitMqConnectionString"] = 
            new ConnectionStringReference(rabbitMqServer.Resource, false);
        context.EnvironmentVariables["checkerGameWebAppHost"] = context.EnvironmentVariables["services__rest-api__https__0"];
    })
    .WaitFor(postgresServer)
    .WaitFor(mongoDbServer)
    .WaitFor(rabbitMqServer)
    .WaitFor(api);

builder.Build().Run();