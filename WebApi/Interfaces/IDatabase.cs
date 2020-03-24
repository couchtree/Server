using System;
using System.Collections.Generic;
using Web_Api.Controllers;
using Web_Api.Entities;

namespace Web_Api.Interfaces
{
    public interface IDatabase
    {
        bool Contains(string id);
        void Create(string id);
        void Update(string id, LocationUpdateDTO pos);
        void Delete(string id);
        IEnumerable<NearbyPlayer> GetNearby(string playerId, Location playerLocation);
    }
}