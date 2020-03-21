using System;
using System.Collections.Generic;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Dependencies
{
    public class MemoryDatabase : IDatabase, INearByFinder
    {
        private const double critical_distance = 0.1;
        private readonly Dictionary<Guid, Position> players = new Dictionary<Guid, Position>();

        public bool Contains(Guid id)
        {
            return this.players.ContainsKey(id);
        }

        public void Create(Guid id, Position pos)
        {
            if (this.Contains(id))
                return;
            this.players.Add(id, pos);
        }

        public void Delete(Guid id)
        {
            if (!this.Contains(id))
                return;
            this.players.Remove(id);
        }

        public void Update(Guid id, Position pos)
        {
            if (!this.Contains(id))
                return;
            this.players[id] = pos;
        }

        /*
        * Find Users considered nearby the given id
        */
        public Position[] GetNearby(Guid id)
        {
            var nearby = new List<Position>();
            if (!this.Contains(id))
                return nearby.ToArray();

            var me = this.players[id];
            foreach (var player in this.players)
            {
                if (player.Key == id)
                    continue;
                if (Math.Abs(player.Value.Lat - me.Lat) < critical_distance ||
                   Math.Abs(player.Value.Lon - me.Lon) < critical_distance)
                    nearby.Add(player.Value);
            }

            return nearby.ToArray();
        }
    }
}
