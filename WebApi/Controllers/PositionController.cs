using System;
using Microsoft.AspNetCore.Mvc;
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

    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IDatabase db;
        private readonly INearByFinder nearByFinder;

        public PlayerController(IDatabase db, INearByFinder nearByFinder)
        {
            this.db = db;
            this.nearByFinder = nearByFinder;
        }

        [HttpPost("{id}/location")]
        public ActionResult<PositionResponseDTO[]> UpdateLocation(Guid id, [FromBody] PositionDTO pos)
        {
            if (db.Contains(id))
            {
                db.Update(id, new Location { lat = pos.Lat, lon = pos.Long });
            }
            else
            {
                db.Create(id, new Location { lat = pos.Lat, lon = pos.Long });
            }
            return this.nearByFinder.GetNearby(id);
        }
    }
}

