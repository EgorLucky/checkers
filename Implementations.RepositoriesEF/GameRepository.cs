using AutoMapper;
using DomainLogic.Filters;
using DomainLogic.Models;
using DomainLogic.Repositories;
using Implementations.RepositoriesEF.Entitites;
using Microsoft.EntityFrameworkCore;

using DomainGame = DomainLogic.Models.Game;
using Game = Implementations.RepositoriesEF.Entitites.Game;

namespace Implementations.RepositoriesEF
{
    public class GameRepository: IGameRepository
    {
        private readonly GameDbContext _context;
        private readonly IMapper _mapper;

        public GameRepository(GameDbContext gameDbContext, IMapper mapper)
        {
            _context = gameDbContext;
            _mapper = mapper;
        }

        public async Task Create(DomainGame game)
        {
            var dbGame = _mapper.Map<Game>(game);
            var players = new List<Player>();

            if (game.FirstPlayerCode.HasValue)
                players.Add(new Player
                {
                    Id = game.FirstPlayerCode.GetValueOrDefault(),
                    GameId = game.Id,
                    Type = GamePlayer.FirstPlayer
                });
            if (game.SecondPlayerCode.HasValue)
                players.Add(new Player
                {
                    Id = game.SecondPlayerCode.GetValueOrDefault(),
                    GameId = game.Id,
                    Type = GamePlayer.SecondPlayer
                });

            await _context.Games.AddAsync(dbGame);
            await _context.Players.AddRangeAsync(players);

            await _context.SaveChangesAsync();
        }

        public async Task<DomainGame> Get(GameGetFilter filter)
        {
            DomainGame game = default;
            if (filter.Id != null)
            {
                var dbGame = await _context
                    .Games
                    .Include(g => g.Players)
                    .FirstOrDefaultAsync(g => g.Id == filter.Id);
                
                game = MapToDomainGame(dbGame);
            }
            if (filter.FirstPlayerCode != null)
            {
                var dbGame = await _context
                    .Games
                    .Include(g => g.Players)
                    .FirstOrDefaultAsync(g => g.Players.Any(p => p.Id == filter.FirstPlayerCode && p.Type == GamePlayer.FirstPlayer));

                game = MapToDomainGame(dbGame);
            }
            if (filter.PlayerCode != null)
            {
                var dbGame = await _context
                    .Games
                    .Include(g => g.Players)
                    .FirstOrDefaultAsync(g => g.Players.Any(p => p.Id == filter.PlayerCode));

                game = MapToDomainGame(dbGame);
            }
            return game;
        }

        DomainGame MapToDomainGame(Game dbGame)
        {
            var game = _mapper.Map<DomainGame>(dbGame);

            if (game != null)
            {
                game.FirstPlayerCode = dbGame.Players.FirstOrDefault(p => p.Type == GamePlayer.FirstPlayer)?.Id;
                game.SecondPlayerCode = dbGame.Players.FirstOrDefault(p => p.Type == GamePlayer.SecondPlayer)?.Id;
            }

            return game;
        }

        public async Task Update(DomainGame game)
        {
            var dbGame = await _context
                    .Games
                    .Include(g => g.Players)
                    .FirstOrDefaultAsync(g => g.Id == game.Id);

            if (game.FirstPlayerCode.HasValue) 
            {
                var firstPlayer = dbGame.Players.FirstOrDefault(p => p.Type == GamePlayer.FirstPlayer);

                if (firstPlayer is null)
                    await _context.AddAsync(new Player(game.FirstPlayerCode.Value, game.Id, GamePlayer.FirstPlayer));
                else if (firstPlayer.Id != game.FirstPlayerCode)
                {
                    _context.Remove(firstPlayer);
                    await _context.AddAsync(new Player(game.FirstPlayerCode.Value, game.Id, GamePlayer.FirstPlayer));
                }
            }
            if (game.SecondPlayerCode.HasValue)
            {
                var secondPlayer = dbGame.Players.FirstOrDefault(p => p.Type == GamePlayer.SecondPlayer);

                if (secondPlayer is null)
                    await _context.AddAsync(new Player(game.SecondPlayerCode.Value, game.Id, GamePlayer.SecondPlayer));
                else if (secondPlayer.Id != game.SecondPlayerCode)
                {
                    _context.Remove(secondPlayer);
                    await _context.AddAsync(new Player(game.SecondPlayerCode.Value, game.Id, GamePlayer.SecondPlayer));
                }
            }

            _mapper.Map(game, dbGame);
            await _context.SaveChangesAsync();
        }
    }
}