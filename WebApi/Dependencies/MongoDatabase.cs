using System;
using System.Collections.Generic;
using System.Linq;
using Web_Api.Controllers;
using Web_Api.Entities;
using Web_Api.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Web_Api.Dependencies
{
    [BsonIgnoreExtraElements]
    public class PlayerModel
    {
        public Guid guid;
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> currentLocation;
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> previousLocation;
        public bool tracked;
        public bool atHome;
    }

    public class MongoDatabase : IDatabase, INearByFinder
    {
        private readonly Lazy<MongoClient> lazyClient = new Lazy<MongoClient>(() => new MongoClient("mongodb://localhost:27017"));

        private IMongoCollection<PlayerModel> playerCollection
        {
            get
            {
                MongoClient client = lazyClient.Value;
                var db = client.GetDatabase("main");
                var collection = db.GetCollection<PlayerModel>("players");

                // create additional indices for the collection of players
                // (idempotent, only affects db if index has not yet been created)
                collection.Indexes.CreateOne(new CreateIndexModel<PlayerModel>(
                        Builders<PlayerModel>.IndexKeys.Ascending(p => p.guid),
                        new CreateIndexOptions { Unique = true }
                ));
                collection.Indexes.CreateOne(new CreateIndexModel<PlayerModel>(
                        Builders<PlayerModel>.IndexKeys.Geo2DSphere(p => p.currentLocation)
                ));

                return collection;
            }
        }

        public bool Contains(Guid id)
        {
            return playerCollection.Find(Builders<PlayerModel>.Filter.Eq(p => p.guid, id)).CountDocuments() == 1;
        }

        public void Create(Guid id, Location location)
        {
            var geoJsonPoint = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(location.lon, location.lat));
            playerCollection.InsertOne(new PlayerModel
            {
                guid = id,
                currentLocation = geoJsonPoint,
                previousLocation = geoJsonPoint,
                atHome = false, // FIXME: use actual client supplied data
            });
        }

        public void Delete(Guid id)
        {
            playerCollection.DeleteOne(Builders<PlayerModel>.Filter.Eq(p => p.guid, id));
        }

        public void Update(Guid id, Location location)
        {
            playerCollection.UpdateOne(
                Builders<PlayerModel>.Filter.Eq(p => p.guid, id),
                Builders<PlayerModel>.Update
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
        public IEnumerable<Location> GetNearby(Guid id, Location location)
        {
            // FIXME: ignore players that are marked as not tracked
            return playerCollection.Find(
                    Builders<PlayerModel>.Filter.NearSphere(
                        p => p.currentLocation,
                        GeoJson.Point(GeoJson.Geographic(location.lon, location.lat)),
                        100.0
                    )
                )
                .Limit(6)
                .ToEnumerable()
                .Where(p => p.guid != id)
                .Select(p =>
                {
                    var coords = p.currentLocation.Coordinates;
                    return new Location { lon = coords.Longitude, lat = coords.Latitude };
                });
        }
    }
}
