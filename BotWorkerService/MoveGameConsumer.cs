using DomainLogic.Services;
using Implementations.MassTransitMq;
using MassTransit;

namespace BotWorkerService
{
    public class MoveGameConsumer : IConsumer<MoveNotify>
    {
        readonly ILogger<MoveGameConsumer> _logger;
        readonly Bot _bot;
        readonly IServiceScope _scope;

        public MoveGameConsumer(ILogger<MoveGameConsumer> logger, Bot bot, IServiceProvider provider)
        {
            _logger = logger;
            _scope = provider.CreateScope();
            _bot = _scope.ServiceProvider.GetRequiredService<Bot>();
        }

        public Task Consume(ConsumeContext<MoveNotify> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.GameId);

            var message = context.Message;

            Task.Run(async () =>
            {
                await _bot.Move(message.GameId);
                _scope.Dispose();
            });

            return Task.CompletedTask;
        }
    }
}