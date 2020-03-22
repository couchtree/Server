using System;
using System.Collections.Generic;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Dependencies
{
    public class MemoryDatabase : IDatabase, INearByFinder
    {
        private const double critical_distance = 0.1;
        private readonly Dictionary<string, Location> players = new Dictionary<string, Location>();

        public bool Contains(string id)
        {
            return this.players.ContainsKey(id);
        }

        public void Create(string id, Location pos)
        {
            if (this.Contains(id))
                return;
            this.players.Add(id, pos);
        }

        public void Delete(string id)
        {
            if (!this.Contains(id))
                return;
            this.players.Remove(id);
        }

        public void Update(string id, Location pos)
        {
            if (!this.Contains(id))
                return;
            this.players[id] = pos;
        }

        /*
        * Find Users considered nearby the given id
        */
        public IEnumerable<Location> GetNearby(string id, Location location)
        {
            var nearby = new List<Location>();
            if (!this.Contains(id))
                return nearby.ToArray();

            var me = this.players[id];
            foreach (var player in this.players)
            {
                if (player.Key == id)
                    continue;
                if (Math.Abs(player.Value.lat - me.lat) < critical_distance ||
                   Math.Abs(player.Value.lon - me.lon) < critical_distance)
                    nearby.Add(player.Value);
            }

            return nearby.ToArray();
        }
    }
}
