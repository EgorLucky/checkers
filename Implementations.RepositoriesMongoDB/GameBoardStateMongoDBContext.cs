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
            MongoInitializator.Initialize();
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

        public async Task ConfigureIndexesAsync()
        {
            await BoardStates.Indexes.CreateManyAsync(new List<CreateIndexModel<BoardState>>
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

    static class MongoInitializator
    {
        private static bool _initialized = false;
        private static readonly Lock _locker = new();
        internal static void Initialize()
        {
            if (_initialized) return;
            
            lock (_locker)
            {
                if (_initialized) return;
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                _initialized = true;
            }
        }
    }
}