using System;
using System.Collections.Generic;
using Web_Api.Entities;
using Web_Api.Interfaces;
using Web_Api.Controllers;

namespace Web_Api.Dependencies
{
    public class MemoryDatabase : IDatabase
    {
        private readonly Dictionary<string, Location> players = new Dictionary<string, Location>();

        public bool Contains(string id)
        {
            return this.players.ContainsKey(id);
        }

        public void Create(string id)
        {
            if (this.Contains(id))
                return;
            this.players.Add(id, new Location { lat = 0, lon = 0 });
        }

        public void Delete(string id)
        {
            if (!this.Contains(id))
                return;
            this.players.Remove(id);
        }

        public void Update(string id, LocationUpdateDTO ludto)
        {
            if (!this.Contains(id))
                return;
            this.players[id] = new Location { lat = ludto.Lat, lon = ludto.Lon };
        }

        private const double earthRadius = 6371e3;

        private double GeoDist(Location l1, Location l2)
        {
            var phi1 = l1.lat.ToRadians();
            var phi2 = l2.lat.ToRadians();
            var dPhi = (l2.lat - l1.lat).ToRadians();
            var dLambda = (l2.lon - l1.lon).ToRadians();

            var a = Math.Pow(Math.Sin(dPhi / 2), 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Pow(Math.Sin(dLambda / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        /*
        * Find Users considered nearby the given id
        */
        public IEnumerable<NearbyPlayer> GetNearby(string id, Location location)
        {
            var nearby = new List<NearbyPlayer>();
            if (!this.Contains(id))
                return nearby.ToArray();

            var me = this.players[id];
            foreach (var player in this.players)
            {
                if (player.Key == id)
                    continue;

                var other = player.Value;

                var calculatedDist = GeoDist(location, other);

                if (calculatedDist < NearbyPlayer.maxDistance)
                    nearby.Add(new NearbyPlayer { currentLocation = player.Value, calculatedDist = calculatedDist });
            }

            return nearby.ToArray();
        }
    }
}
