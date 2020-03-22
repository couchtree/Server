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
        public bool Contains(string id)
        {
            throw new NotImplementedException();
        }

        public void Create(string id, Location pos)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetNearby(string id, Location location) => throw new NotImplementedException();

        public void Update(string id, Location pos)
        {
            throw new NotImplementedException();
        }


    }
}
