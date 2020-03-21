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
        public float Lat;
        public float Long;
        public bool at_home;
    }

    public class PositionResponseDTO
    {
        public int dir;
        public float dist;
        public float vel_nearing;

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
        public JsonResult Put(Guid id, [FromBody] PositionDTO pos)
        {
            if (db.Contains(id))
            {
                db.Update(id, new Position { Lat = pos.Lat, Lon = pos.Long });
            }
            else
            {
                db.Create(id, new Position { Lat = pos.Lat, Lon = pos.Long });
            }
            var resp = GetNearby(id);
            return new JsonResult(resp) { StatusCode = 200 };

        }

        private PositionResponseDTO GetNearby(Guid id)
        {
            return new PositionResponseDTO();
        }
    }

}

