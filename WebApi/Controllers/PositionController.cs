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

    [ApiController]
    [Route("_api/[controller]")]
    public class PositionController : ControllerBase
    {
        [HttpPut("{id}")]
        public JsonResult Put(int id, [FromBody] Position pos, IDatabase db)
        {
            if (db.Contains(id))
            {
                db.Insert(id, pos);
                return new JsonResult("ok") { StatusCode = 200 };
            }
            else
            {
                db.Update(id, pos);
            }
            return new JsonResult("error") { StatusCode = 404 };
        }

    }
}

