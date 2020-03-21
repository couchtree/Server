using System;
using Web_Api.Entities;

namespace Web_Api.Interfaces
{
    public interface INearByFinder
    {
        Location[] GetNearby(Guid id);
    }
}
