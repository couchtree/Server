using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Controllers
{
    public class LocationUpdateDTO
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public long Timestamp { get; set; }
        public bool AtHome { get; set; }
        public bool Tracked { get; set; }
    }

    public class LocationUpdateResponseDTO
    {
        public LocationUpdateResponseNearbyPlayerDTO[] nearby_players { get; set; }
    }

    public class LocationUpdateResponseNearbyPlayerDTO
    {
        public int Dir { get; set; }
        public double Dist { get; set; }
        public double VelNearing { get; set; }
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
        private readonly IDirectionCalculator dir;

        public PlayerController(IDatabase db, IDirectionCalculator dirCalculator)
        {
            this.dir = dirCalculator;
            this.db = db;
        }

        [HttpPost("{id}/location")]
        public ActionResult<LocationUpdateResponseDTO> UpdateLocation(string id, [FromBody] LocationUpdateDTO ludto)
        {
            var newPosition = new Location { lat = ludto.Lat, lon = ludto.Lon };
            if (!db.Contains(id)) db.Create(id);
            db.Update(id, ludto);

            var nearby = db.GetNearby(id, newPosition);
            return new LocationUpdateResponseDTO
            {
                nearby_players = nearby.Select((nearBy) => this.CreateNearbyPlayerDTO(newPosition, nearBy)).ToArray()
            };
        }

        public class CreateResponseDTO
        {
            public string id { get; set; }
        }

        [HttpGet("create")]
        public ActionResult<CreateResponseDTO> Create()
        {
            string newId;
            do
            {
                newId = Guid.NewGuid().ToString();
            } while (db.Contains(newId));
            db.Create(newId);

            return new CreateResponseDTO
            {
                id = newId
            };
        }

        private LocationUpdateResponseNearbyPlayerDTO CreateNearbyPlayerDTO(Location me, NearbyPlayer other)
        {
            return new LocationUpdateResponseNearbyPlayerDTO()
            {
                Dist = other.calculatedDist,
                Dir = (int)dir.CalculateDirection(me, other.currentLocation),
                VelNearing = 3 // FIXME: actually calculate velocity of other player towards me, or remove from API
            };
        }
    }
}

