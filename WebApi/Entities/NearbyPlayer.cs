using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Api.Entities
{
    public class NearbyPlayer
    {
        public const double maxDistance = 100;

        public Location currentLocation;
        public long currentTimestamp;
        public Location previousLocation;
        public long previousTimestamp;

        /// <summary>Distance to the nearby player in meters</summary>
        public double calculatedDist;
    }
}
