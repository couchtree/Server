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
    
    public PositionResponseDTO[] GetNearby(Guid id) 
    {
      return new PositionResponseDTO[] { new PositionResponseDTO() };
    }
  }
}
