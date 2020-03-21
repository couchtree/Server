using System;
using Web_Api.Controllers;
using Web_Api.Entities;
using Web_Api.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Web_Api.Dependencies
{

    public class MongoDatabase : IDatabase, INearByFinder
    {
        private readonly Lazy<MongoClient> lazyClient = new Lazy<MongoClient>(() => new MongoClient("mongodb://localhost:27017"));

        private IMongoCollection<Player> playerCollection
        {
            get
            {
                MongoClient client = lazyClient.Value;
                var db = client.GetDatabase("main");
                var collection = db.GetCollection<Player>("players");
                // create a unique index over the guids
                // (idempotent, only affects db if index has not yet been created)
                collection.Indexes.CreateOne(new CreateIndexModel<Player>(
                        Builders<Player>.IndexKeys.Ascending(p => p.guid),
                        new CreateIndexOptions { Unique = true }
                ));
                return collection;
            }
        }

        public bool Contains(Guid id)
        {
            return playerCollection.Find(Builders<Player>.Filter.Eq(p => p.guid, id)).CountDocuments() == 1;
        }

        public void Create(Guid id, Location location)
        {
            Update(id, location);
        }

        public void Delete(Guid id)
        {
            playerCollection.DeleteOne(Builders<Player>.Filter.Eq(p => p.guid, id));
        }

        public void Update(Guid id, Location location)
        {
            playerCollection.UpdateOne(
                Builders<Player>.Filter.Eq(p => p.guid, id),
                new UpdateDefinitionBuilder<Player>()
                    .Set("previousLocation", "$currentLocation")
                    .Set("currentLocation", location)
            );
        }

        /*
        * Find Users considered nearby the given id
        */
        public Location[] GetNearby(Guid id)
        {
            // FIXME: STUB
            Location[] nearby = new Location[0];
            return nearby;
        }
    }
}
