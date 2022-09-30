using DomainLogic.Services;
using Implementations.MassTransitMq;
using MassTransit;

public class RegisterGameConsumer : IConsumer<RegisterNotify>
{
    readonly ILogger<RegisterGameConsumer> _logger;
    readonly Bot _bot;
    readonly IServiceScope _scope;

    public RegisterGameConsumer(ILogger<RegisterGameConsumer> logger, Bot bot, IServiceProvider provider)
    {
        _logger = logger;
        _scope = provider.CreateScope();
        _bot = _scope.ServiceProvider.GetRequiredService<Bot>();
    }

    public Task Consume(ConsumeContext<RegisterNotify> context)
    {
        _logger.LogInformation("Received Text: {Text}", context.Message.GameId);

        var message = context.Message;

        Task.Run(async () =>
        {
            await _bot.Register(message.GameId);
            _scope.Dispose();
        });

        return Task.CompletedTask;
    }
}