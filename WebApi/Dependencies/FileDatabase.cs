using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Api.Controllers;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Dependencies
{
    public class FileDatabase : IDatabase, INearByFinder
    {
        public bool Contains(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Create(Guid id, Location pos)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetNearby(Guid id, Location location) => throw new NotImplementedException();

        public void Update(Guid id, Location pos)
        {
            throw new NotImplementedException();
        }


    }
}
