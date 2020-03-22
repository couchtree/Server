using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Controllers
{
    public class LocationUpdateDTO
    {
        public float Lat { get; set; }
        public float Long { get; set; }
        public bool at_home { get; set; }
        public bool Tracked { get; set; }
    }

    public class LocationUpdateResponseDTO
    {
        public LocationUpdateResponseNearbyPlayerDTO[] nearby_players { get; set; }
    }

    public class LocationUpdateResponseNearbyPlayerDTO
    {
        public int dir { get; set; }
        public float dist { get; set; }
        public float vel_nearing { get; set; }
    }

    public enum Directions
    {
        North = 0,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
    }

    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IDatabase db;
        private readonly INearByFinder nearByFinder;
        private readonly IDirectionCalculator dir;

        public PlayerController(IDatabase db, INearByFinder nearByFinder, IDirectionCalculator dirCalculator)
        {
            this.dir = dirCalculator;
            this.db = db;
            this.nearByFinder = nearByFinder;
        }

        [HttpPost("{id}/location")]
        public ActionResult<LocationUpdateResponseDTO> UpdateLocation(string id, [FromBody] LocationUpdateDTO pos)
        {
            var newPosition = new Location { lat = pos.Lat, lon = pos.Long, tracked = pos.Tracked, at_home = pos.at_home };
            if (db.Contains(id))
            {
                db.Update(id, newPosition);
            }
            else
            {
                db.Create(id, newPosition);
            }

            var nearby = this.nearByFinder.GetNearby(id, newPosition);
            return new LocationUpdateResponseDTO
            {
                nearby_players = nearby.Select((nearBy) => this.CreateNearbyPlayerDTO(newPosition, nearBy)).ToArray()
            };
        }

        private LocationUpdateResponseNearbyPlayerDTO CreateNearbyPlayerDTO(Location me, Location other)
        {
            var latDistance = me.lat - other.lat;
            var lonDistance = me.lon - other.lon;
            var totalDistance = Math.Sqrt(Math.Pow(latDistance, 2) + Math.Pow(lonDistance, 2));

            return new LocationUpdateResponseNearbyPlayerDTO()
            {
                dist = (float)totalDistance,
                dir = (int)dir.CalculateDirection(me, other),
                vel_nearing = 3
            };
        }
    }
}

