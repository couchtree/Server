using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Web_Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Player
    {
        public string guid;
        public Location currentLocation;
        public Location previousLocation;
        public bool tracked;
        public bool atHome;
    }
}
