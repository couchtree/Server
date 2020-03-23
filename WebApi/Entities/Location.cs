using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Api.Entities
{
    public class Location
    {
        public double lat;
        public double lon;
        public bool tracked;
        public bool at_home;

        /// <summary>
        /// represents the number of milliseconds that have elapsed since 1970-01-01T00:00:00.000Z
        /// </summary>
        public long timestamp;
    }
}
