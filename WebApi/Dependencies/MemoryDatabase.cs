using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Api.Controllers;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Dependencies
{
    public class MemoryDatabase : IDatabase, INearByFinder
    {
        private readonly Dictionary<Guid, Location> players = new Dictionary<Guid, Location>();

        public bool Contains(Guid id)
        {
            return this.players.ContainsKey(id);
        }

        public void Create(Guid id, Location pos)
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

        public void Update(Guid id, Location pos)
        {
            if (!this.Contains(id))
                return;
            this.players[id] = pos;
        }

        /*
        * Find Users considered nearby the given id
        */
        public PositionResponseDTO[] GetNearby(Guid id)
        {
            PositionResponseDTO[] nearby = new PositionResponseDTO[3];
            // TODO Calculate Nearby / Get Nearby from Database
            for (int i = 0; i < nearby.Length; i++)
            {
                nearby[i] = new PositionResponseDTO();
            }

            return nearby;
        }
    }
}
