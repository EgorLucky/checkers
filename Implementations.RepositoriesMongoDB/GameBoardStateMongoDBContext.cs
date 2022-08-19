using DomainLogic.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Implementations.RepositoriesMongoDB
{
    public class GameBoardStateMongoDBContext: MongoClient
    {
        private const string _databaseName = "checkers";
        private readonly IMongoDatabase _database;
        private IMongoCollection<BoardState> _boardStates;

        public GameBoardStateMongoDBContext(MongoClientSettings settings) : base(settings) 
        {
            _database = this.GetDatabase(_databaseName);
        }

        public IMongoCollection<BoardState> BoardStates 
        {
            get
            { 
                if(_boardStates == null)
                    _boardStates = _database.GetCollection<BoardState>(nameof(BoardStates));
                return _boardStates;
            } 
        }

        public void ConfigureIndexes()
        {
            var boardStateIndexCreateResult = BoardStates.Indexes.CreateMany(new List<CreateIndexModel<BoardState>>
            {
                new CreateIndexModel<BoardState>(
                    new BsonDocumentIndexKeysDefinition<BoardState>(
                        new BsonDocument(new Dictionary<string, object>()
                        {
                            { nameof(BoardState.GameId), 1 },
                        })
                    )
                )
            });
        }
    }
}