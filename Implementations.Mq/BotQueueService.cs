using DomainLogic.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Implementations.Mq
{
    public class BotQueueService : IBotNotifier
    {
        private readonly Bot _bot;
        private static bool RegistrationConsumingStarted;
        private static bool MoveConsumingStarted;

        public BotQueueService(Bot bot)
        {
            _bot = bot;
        }

        static ConcurrentQueue<Guid> RegisterQueue = new ConcurrentQueue<Guid>();
        static ConcurrentQueue<Guid> MoveQueue = new ConcurrentQueue<Guid>();

        public async Task RegisterNotify(Guid gameId)
        {
            RegisterQueue.Enqueue(gameId);

            if (RegistrationConsumingStarted == false)
            {
                StartRegistrationConsuming(new CancellationToken());
                RegistrationConsumingStarted = true;
            }
        }

        public async void StartRegistrationConsuming(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (RegisterQueue.TryDequeue(out Guid gameId))
                {
                    await _bot.Register(gameId);
                }
            }
        }

        public async Task MoveNotify(Guid gameId)
        {
            MoveQueue.Enqueue(gameId);

            if (MoveConsumingStarted == false)
            {
                StartMoveConsuming(new CancellationToken());
                MoveConsumingStarted = true;
            }
        }

        private async void StartMoveConsuming(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (MoveQueue.TryDequeue(out Guid gameId))
                {
                    await _bot.Move(gameId);
                }
            }
        }
    }
}
