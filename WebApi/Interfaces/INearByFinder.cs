using System;
using Web_Api.Entities;
using System.Collections.Generic;

namespace Web_Api.Interfaces
{
    public interface INearByFinder
    {
        IEnumerable<Location> GetNearby(string playerId, Location playerLocation);
    }
}
