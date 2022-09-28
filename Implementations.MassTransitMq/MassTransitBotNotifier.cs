using DomainLogic.Services;
using MassTransit;

namespace Implementations.MassTransitMq
{
    public class MassTransitBotNotifier : IBotNotifier
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitBotNotifier(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task MoveNotify(Guid gameId)
        {
            return _publishEndpoint.Publish(new MoveNotify(GameId: gameId));
        }

        public Task RegisterNotify(Guid gameId)
        {
            return _publishEndpoint.Publish(new RegisterNotify(GameId: gameId));
        }
    }

    public record MoveNotify(Guid GameId);
    public record RegisterNotify(Guid GameId);
    public record RabbitMqConfig(
        string Host,
        string Username,
        string Password,
        string VirtualHost);
}