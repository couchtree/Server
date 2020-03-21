using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Controllers
{
    public class PositionDTO
    {
        public float Lat { get; set; }
        public float Long { get; set; }
        public bool at_home { get; set; }
    }

    public class PositionResponseDTO
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
        public ActionResult<PositionResponseDTO[]> UpdateLocation(Guid id, [FromBody] PositionDTO pos)
        {
            var newPosition = new Location { lat = pos.Lat, lon = pos.Long };
            if (db.Contains(id))
            {
                db.Update(id, newPosition);
            }
            else
            {
                db.Create(id, newPosition);
            }

            var nearby = this.nearByFinder.GetNearby(id);
            return nearby.Select((nearBy) => this.CreateResponseDTO(newPosition, nearBy)).ToArray();
        }

        private PositionResponseDTO CreateResponseDTO(Location me, Location other)
        {
            var latDistance = me.lat - other.lat;
            var lonDistance = me.lon - other.lon;
            var totalDistance = Math.Sqrt(Math.Pow(latDistance, 2) + Math.Pow(lonDistance, 2));

            return new PositionResponseDTO()
            {
                dist = (float)totalDistance,
                dir = (int)dir.CalculateDirection(me, other),
                vel_nearing = 3
            };
        }
    }
}

