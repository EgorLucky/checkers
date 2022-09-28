using BotWorkerService;
using MassTransit;
using System.Text.Json;
using Implementations.MassTransitMq;
using Implementations.RepositoriesEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DomainLogic.Repositories;
using DomainLogic.Services;
using Implementations.ArtificialAnalyzerRandom;
using Implementations.GameServiceHttpClient;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddMassTransit(x =>
        {
            var rabbitMqConfigJson = configuration.GetValue<string>("checkerGameRabbitMqConfig");
            var RabbitMqConfig = JsonSerializer.Deserialize<RabbitMqConfig>(rabbitMqConfigJson);

            x.AddConsumer<RegisterGameConsumer>();
            x.AddConsumer<MoveGameConsumer>();
            x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.Host(RabbitMqConfig.Host, RabbitMqConfig.VirtualHost, h =>
                {
                    h.Username(RabbitMqConfig.Username);
                    h.Password(RabbitMqConfig.Password);

                });

                config.ReceiveEndpoint(nameof(RegisterNotify), ep =>
                {
                    ep.PrefetchCount = 16;
                    //ep.UseMessageRetry(r => r.Interval(2, 100));
                    ep.ConfigureConsumer<RegisterGameConsumer>(provider);
                });

                config.ReceiveEndpoint(nameof(MoveNotify), ep =>
                {
                    ep.PrefetchCount = 16;
                    //ep.UseMessageRetry(r => r.Interval(2, 100));
                    ep.ConfigureConsumer<MoveGameConsumer>(provider);
                });
            }));
        });
        
        services.AddDbContext<GameDbContext>(options => options.UseNpgsql(configuration.GetValue<string>("checkerGameConnectionString")));
        services.AddScoped<Bot>()
                .AddScoped<IBotRepository, BotRepository>()
                .AddSingleton<IArtificialGameAnalyzer, RandomArtificialGameAnalyzer>()
                .AddHttpClient<IGameServiceClient, GameServiceHttpClient>(c => c.BaseAddress = new Uri("https://localhost:5001"));
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
