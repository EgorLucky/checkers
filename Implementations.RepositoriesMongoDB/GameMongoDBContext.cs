using DomainLogic.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Implementations.RepositoriesMongoDB
{
    public class GameMongoDBContext: MongoClient, IDisposable
    {
        private const string _databaseName = "checkers";
        private readonly IMongoDatabase _database;
        public GameMongoDBContext(MongoClientSettings settings) : base(settings) 
        {
            _database = this.GetDatabase(_databaseName);
        }

        public IMongoCollection<Game> Games => _database.GetCollection<Game>(nameof(this.Games));

        public IMongoCollection<BoardState> BoardStates => _database.GetCollection<BoardState>(nameof(this.BoardStates));

        public void Dispose()
        {
            ClusterRegistry.Instance.UnregisterAndDisposeCluster(this.Cluster);
        }
    }
}