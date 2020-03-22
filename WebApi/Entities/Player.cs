using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Api.Entities
{
    public class Player
    {
        public string id;
        public Location currentLocation;
        public Location previousLocation;
        public bool tracked;
        public bool atHome;
    }
}
