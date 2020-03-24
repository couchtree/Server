using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Web_Api.Entities
{
    public class Location
    {
        public double lat;
        public double lon;

        public static implicit operator Location(GeoJsonPoint<GeoJson2DGeographicCoordinates> gjp)
            => new Location
            {
                lat = gjp.Coordinates.Latitude,
                lon = gjp.Coordinates.Longitude
            };

        public static implicit operator GeoJsonPoint<GeoJson2DGeographicCoordinates>(Location loc)
            => GeoJson.Point(GeoJson.Geographic(loc.lon, loc.lat));
    }
}
