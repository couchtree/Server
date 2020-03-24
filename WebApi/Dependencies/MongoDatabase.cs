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
        public long creationTime;
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> currentLocation;
        public long currentTimestamp;
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> previousLocation;
        public long previousTimestamp;
        public bool tracked;
        public bool atHome;
    }

    public class MongoDatabase : IDatabase
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

        public void Create(string id)
        {
            var crt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var geoJsonPoint = GeoJson.Point(GeoJson.Geographic(0, 0));
            playerCollection.InsertOne(
                new PlayerModel
                {
                    id = id,
                    creationTime = crt,
                    currentLocation = geoJsonPoint,
                    currentTimestamp = crt,
                    previousLocation = geoJsonPoint,
                    previousTimestamp = crt,
                    atHome = false,
                    tracked = false,
                });
        }

        public void Delete(string id)
        {
            playerCollection.DeleteOne(Builders<PlayerModel>.Filter.Eq(p => p.id, id));
        }

        public void Update(string id, LocationUpdateDTO ludto)
        {
            PipelineDefinition<PlayerModel, PlayerModel> pipeline = new List<BsonDocument>() {
                new BsonDocument(new BsonElement(
                    "$set",
                    new BsonDocument(new List<BsonElement>() {
                        new BsonElement("previousLocation", "$currentLocation"),
                        new BsonElement("previousTimestamp", "$currentTimestamp"),
                        new BsonElement("currentLocation", GeoJson.Point(GeoJson.Geographic(ludto.Lon, ludto.Lat)).ToBsonDocument()),
                        new BsonElement("currentTimestamp", ludto.Timestamp),
                        new BsonElement("atHome", ludto.AtHome),
                        new BsonElement("tracked", ludto.Tracked)
                    })
                ))
            };
            playerCollection.UpdateOne(Builders<PlayerModel>.Filter.Eq(p => p.id, id), pipeline);
        }

        [BsonIgnoreExtraElements]
        public class NearbyPlayerModel
        {
            public GeoJsonPoint<GeoJson2DGeographicCoordinates> currentLocation;
            public long currentTimestamp;
            public GeoJsonPoint<GeoJson2DGeographicCoordinates> previousLocation;
            public long previousTimestamp;
            public double calculatedDist;
        }

        /*
        * Find Users considered nearby the given id
        */
        public IEnumerable<NearbyPlayer> GetNearby(string id, Location location)
        {
            return playerCollection.Aggregate().AppendStage<BsonDocument>(
                new BsonDocument(new BsonElement(
                    "$geoNear",
                    new BsonDocument(new List<BsonElement>() {
                        new BsonElement("key", "currentLocation"),
                        new BsonElement("near", GeoJson.Point(GeoJson.Geographic(location.lon, location.lat)).ToBsonDocument()),
                        new BsonElement("distanceField", "calculatedDist"),
                        new BsonElement("maxDistance", NearbyPlayer.maxDistance),
                        new BsonElement("query",
                            new BsonDocument(new BsonElement("$and", new BsonArray(new List<BsonDocument>() {
                                    new BsonDocument(new BsonElement("id", new BsonDocument(new BsonElement("$ne", id)))),
                                    new BsonDocument(new BsonElement("tracked", true))
                            }))
                        ))
                    })
                ))
            ).AppendStage<NearbyPlayerModel>(
                new BsonDocument(new BsonElement(
                    "$project",
                    new BsonDocument(new List<BsonElement>() {
                        new BsonElement("currentLocation", 1),
                        new BsonElement("currentTimestamp", 1),
                        new BsonElement("previousLocation", 1),
                        new BsonElement("previousTimestamp", 1),
                        new BsonElement("calculatedDist", 1),
                    })
                ))
            ).Limit(5).ToEnumerable().Select(p =>
            {
                return new NearbyPlayer
                {
                    currentLocation = p.currentLocation,
                    currentTimestamp = p.currentTimestamp,
                    previousLocation = p.previousLocation,
                    previousTimestamp = p.previousTimestamp,
                    calculatedDist = p.calculatedDist
                };
            });
        }
    }
}
