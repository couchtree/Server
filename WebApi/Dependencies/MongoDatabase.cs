using System;
using System.Collections.Generic;
using System.Linq;
using Web_Api.Controllers;
using Web_Api.Entities;
using Web_Api.Interfaces;
using MongoDB.Driver;

namespace Web_Api.Dependencies
{

    public class MongoDatabase : IDatabase, INearByFinder
    {
        private readonly Lazy<MongoClient> lazyClient;

        private IMongoCollection<Player> playerCollection
        {
            get
            {
                MongoClient client = lazyClient.Value;
                var db = client.GetDatabase("main");
                var collection = db.GetCollection<Player>("players");

                // create additional indices for the collection of players
                // (idempotent, only affects db if index has not yet been created)
                collection.Indexes.CreateOne(new CreateIndexModel<Player>(
                        Builders<Player>.IndexKeys.Ascending(p => p.guid),
                        new CreateIndexOptions { Unique = true }
                ));
                collection.Indexes.CreateOne(new CreateIndexModel<Player>(
                        Builders<Player>.IndexKeys.Geo2DSphere(p => p.currentLocation)
                ));

                return collection;
            }
        }

        public MongoDatabase(string hostname)
        {
            lazyClient = new Lazy<MongoClient>(() => new MongoClient($"mongodb://{hostname}:27017"));
        }

        public bool Contains(string id)
        {
            return playerCollection.Find(Builders<Player>.Filter.Eq(p => p.guid, id)).CountDocuments() == 1;
        }

        public void Create(string id, Location location)
        {
            playerCollection.InsertOne(new Player
            {
                guid = id,
                currentLocation = location,
                previousLocation = location,
                atHome = false, // FIXME: use actual client supplied data
            });
        }

        public void Delete(string id)
        {
            playerCollection.DeleteOne(Builders<Player>.Filter.Eq(p => p.guid, id));
        }

        public void Update(string id, Location location)
        {
            playerCollection.UpdateOne(
                Builders<Player>.Filter.Eq(p => p.guid, id),
                Builders<Player>.Update
                    .Set("previousLocation", "$currentLocation")
                    .Set("currentLocation", location),
                new UpdateOptions { IsUpsert = true }
            );
            Console.WriteLine(id);
            Console.WriteLine(location);
        }

        /*
        * Find Users considered nearby the given id
        */
        public IEnumerable<Location> GetNearby(string id, Location location)
        {
            return playerCollection.Find(
                    Builders<Player>.Filter.NearSphere(p => p.currentLocation, location.lat, location.lon, 100.0)
                )
                .Limit(6)
                .ToEnumerable()
                .Where(p => p.guid != id)
                .Select(p => p.currentLocation);
        }
    }
}
