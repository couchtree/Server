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
    /*
     * #[put("/<id>", data = "<msg>")]
        fn update(db: &Db, id: Id, msg: Json<Message>) -> JsonValue {
            if db.contains_key(&id) {
                db.insert(id, &msg.contents);
                json!({ "status": "ok" })
            } else {
                json!({ "status": "error" })
            }
        }
     */
    public class PositionDTO
    {
        public float Lat;
        public float Long;
    }

    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlayerController : ControllerBase
    {
        [HttpPut("{id}/location")]
        public JsonResult Put(Guid id, [FromBody] PositionDTO pos, IDatabase db)
        {
            if (db.Contains(id))
            {
                db.Update(id, new Position { Lat = pos.Lat, Lon = pos.Long });
                return new JsonResult("ok") { StatusCode = 200 };
            }
            else
            {
                db.Create(id, new Position { Lat = pos.Lat, Lon = pos.Long });
            }
            return new JsonResult("error") { StatusCode = 500 };
        }

        [HttpGet("{id}/nearby")]
        public JsonResult Get(Guid id)
        {
            return new JsonResult(GetNearby(id)) { StatusCode = 200 };
        }

    }

}

