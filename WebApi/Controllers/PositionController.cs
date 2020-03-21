using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public PlayerController(IDatabase db) 
        {
          this.db = db;
        }

        [HttpPost("{id}/location")]
        public ActionResult<PositionResponseDTO[]> UpdateLocation(Guid id, [FromBody] PositionDTO pos)
        {
            if (db.Contains(id))
            {
                db.Update(id, new Position { Lat = pos.Lat, Lon = pos.Long });
            }
            else
            {
                db.Create(id, new Position { Lat = pos.Lat, Lon = pos.Long });
            }
            return GetNearby(id);
        }

        private PositionResponseDTO[] GetNearby(Guid id)
        {
            return new PositionResponseDTO[] { new PositionResponseDTO()};
        }
    }

}

