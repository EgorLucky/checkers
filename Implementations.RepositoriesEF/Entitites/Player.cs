using DomainLogic.Models;

namespace Implementations.RepositoriesEF.Entitites
{
    public class Player
    {
        public Player() { }
        public Player(Guid id, Guid gameId, GamePlayer type)
        {
            Id = id;
            GameId = gameId;
            Type = type;
        }

        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public GamePlayer Type { get; set; }
    }
}