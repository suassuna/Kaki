using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kaki.Models
{
    public class PomodoroMongoProvider : IPomodoroProvider
    {
        public PomodoroMongoProvider()
        {
            Client = new MongoClient();
            Database = Client.GetDatabase(DATABASE_NAME);
            PomodoroBsonConverter = new PomodoroBsonConverter();
        }

        private readonly IMongoClient Client;
        private readonly IMongoDatabase Database;
        private readonly IPomodoroBsonConverter PomodoroBsonConverter;
        private const string DATABASE_NAME = "kaki";
        private const string COLLECTION_NAME = "pomodoros";

        public void AddPomodoro(Pomodoro pomodoro)
        {
            if (pomodoro == null)
                throw new ArgumentNullException(nameof(pomodoro));

            var collection = Database.GetCollection<BsonDocument>(COLLECTION_NAME);
            var document = PomodoroBsonConverter.ConvertToBsonDocument(pomodoro);
            collection.InsertOne(document);
            pomodoro.Key = document["_id"].AsObjectId.ToString();
        }

        public IEnumerable<Pomodoro> GetAllPomodoros()
        {
            var collection = Database.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = new BsonDocument();

            return _GetAllPomodoros(collection, filter).Result;
        }

        private async Task<IEnumerable<Pomodoro>> _GetAllPomodoros(IMongoCollection<BsonDocument> collection, FilterDefinition<BsonDocument> filter)
        {
            var pomodoros = new List<Pomodoro>();

            using (var cursor = await collection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;

                    if(batch != null && batch.Any())
                    {
                        if (!pomodoros.Any())
                            pomodoros = new List<Pomodoro>();

                        foreach (var document in batch)
                            pomodoros.Add(PomodoroBsonConverter.ConvertToPomodoro(document));
                    }
                }
            }

            return pomodoros;
        }

        public Pomodoro GetPomodoro(string key)
        {
            var collection = Database.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(key));

            return _GetAllPomodoros(collection, filter).Result.FirstOrDefault();
        }

        public async void RemovePomodoro(string key)
        {
            var collection = Database.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(key));

            await collection.DeleteOneAsync(filter);
        }

        public async void ResetPomodoro(string key)
        {
            var collection = Database.GetCollection<BsonDocument>(COLLECTION_NAME);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(key));
            var update = Builders<BsonDocument>.Update.Set("startTime", DateTime.UtcNow);

            await collection.UpdateOneAsync(filter, update);
        }
    }
}
