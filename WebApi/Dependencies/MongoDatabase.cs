using System;
using System.Collections.Generic;
using System.Linq;
using Web_Api.Controllers;
using Web_Api.Entities;
using Web_Api.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Web_Api.Dependencies
{
    [BsonIgnoreExtraElements]
    public class PlayerModel
    {
        [BsonRepresentation(BsonType.String)]
        public string id;
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> currentLocation;
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> previousLocation;
        public bool tracked;
        public bool atHome;
    }

    public class MongoDatabase : IDatabase, INearByFinder
    {
        private readonly Lazy<MongoClient> lazyClient;

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
                        Builders<PlayerModel>.IndexKeys.Geo2DSphere(p => p.currentLocation)
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
            return playerCollection.Find(Builders<PlayerModel>.Filter.Eq(p => p.id, id)).CountDocuments() == 1;
        }

        public void Create(string id, Location location)
        {
            var geoJsonPoint = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(location.lon, location.lat));
            playerCollection.InsertOne(new PlayerModel
            {
                id = id,
                currentLocation = geoJsonPoint,
                previousLocation = geoJsonPoint,
                atHome = location.at_home,
                tracked = location.tracked,
            });
        }

        public void Delete(string id)
        {
            playerCollection.DeleteOne(Builders<PlayerModel>.Filter.Eq(p => p.id, id));
        }

        public void Update(string id, Location location)
        {
            PipelineDefinition<PlayerModel, PlayerModel> pipeline = new List<BsonDocument>() {
                new BsonDocument(new BsonElement(
                        "$set",
                        new BsonDocument(new BsonElement("previousLocation", "$currentLocation"))
                )),
                new BsonDocument(new BsonElement(
                    "$set",
                    new BsonDocument(new BsonElement("currentLocation", GeoJson.Point(GeoJson.Geographic(location.lon, location.lat)).ToBsonDocument()))
                ))
            };
            playerCollection.UpdateOne(Builders<PlayerModel>.Filter.Eq(p => p.id, id), pipeline);
        }

        /*
        * Find Users considered nearby the given id
        */
        public IEnumerable<Location> GetNearby(string id, Location location)
        {
            return playerCollection.Find(
                    Builders<PlayerModel>.Filter.NearSphere(
                        p => p.currentLocation,
                        GeoJson.Point(GeoJson.Geographic(location.lon, location.lat)),
                        100.0
                    )
                )
                .Limit(6)
                .ToEnumerable()
                .Where(p => p.id != id && p.tracked)
                .Select(p =>
                {
                    var coords = p.currentLocation.Coordinates;
                    return new Location { lon = coords.Longitude, lat = coords.Latitude };
                });
        }
    }
}
