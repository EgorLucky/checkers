using DomainLogic.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Implementations.RepositoriesMongoDB
{
    public class GameBoardStateMongoDBContext
    {
        private const string DATABASE_NAME = "checkers";
        
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;

        public GameBoardStateMongoDBContext(IMongoClient mongoMongoClient)
        {
            _mongoClient = mongoMongoClient;
            _database = _mongoClient.GetDatabase(DATABASE_NAME);
        }

        public IMongoCollection<BoardState> BoardStates 
        {
            get
            {
                field ??= _database.GetCollection<BoardState>(nameof(BoardStates));
                return field;
            } 
        }

        public void ConfigureIndexes()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BoardStates.Indexes.CreateMany(new List<CreateIndexModel<BoardState>>
            {
                new (
                    new BsonDocumentIndexKeysDefinition<BoardState>(
                        new BsonDocument(new Dictionary<string, object>()
                        {
                            { nameof(BoardState.GameId), 1 },
                        })
                    )
                ),
                new (
                    new BsonDocumentIndexKeysDefinition<BoardState>(
                        new BsonDocument(new Dictionary<string, object>()
                        {
                            { nameof(BoardState.PreviousBoardStateId), 1 },
                        })
                    ),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }
                )
            });
        }
    }
}