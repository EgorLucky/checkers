using System.Text.Json;
using System.Text.Json.Serialization;
using BotWorkerService;
using MassTransit;
using Implementations.MassTransitMq;
using Implementations.RepositoriesEF;
using Microsoft.EntityFrameworkCore;
using DomainLogic.Repositories;
using DomainLogic.Services;
using Implementations.ArtificialAnalyzerRandom;
using Implementations.GameServiceHttpClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Refit;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        
        var postgresConnectionString = configuration.GetValue<string>("checkerGameConnectionString");
        var rabbitMqConnectionString = configuration.GetValue<string>("checkerGameRabbitMqConnectionString");
        var webAppHostUri = configuration.GetValue<string>("checkerGameWebAppHost");
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss:fff} {Level:u3}] [{ThreadId}] {Message}{NewLine}{Exception}")
            .CreateLogger();
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<RegisterGameConsumer>();
            x.AddConsumer<MoveGameConsumer>();
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(rabbitMqConnectionString);

                config.ReceiveEndpoint(nameof(RegisterNotify), ep =>
                {
                    ep.PrefetchCount = 16;
                    //ep.UseMessageRetry(r => r.Interval(2, 100));
                    ep.ConfigureConsumer<RegisterGameConsumer>(context);
                });

                config.ReceiveEndpoint(nameof(MoveNotify), ep =>
                {
                    ep.PrefetchCount = 16;
                    //ep.UseMessageRetry(r => r.Interval(2, 100));
                    ep.ConfigureConsumer<MoveGameConsumer>(context);
                });
            });
        });
        
        services.AddDbContext<GameDbContext>(options => options.UseNpgsql(postgresConnectionString));
        services.AddScoped<Bot>()
                .AddScoped<IBotRepository, BotRepository>()
                .AddSingleton<IArtificialGameAnalyzer, RandomArtificialGameAnalyzer>()
                .AddScoped<IGameServiceClient, GameServiceHttpClient>()
                .AddRefitClient<IGameServiceRefitHttpClient>(new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(
                        new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =  JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter() }
                    })
                })
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(webAppHostUri);
                });
        services.AddAutoMapper(expr => expr.AddProfile<MappingProfile>());
        services.AddHostedService<Worker>();
        services.AddSerilog();
    })
    .Build();

await host.RunAsync();
