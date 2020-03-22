using System;
using Web_Api.Entities;

namespace Web_Api.Interfaces
{
    public interface IDatabase
    {
        bool Contains(string id);
        void Create(string id, Location pos);
        void Update(string id, Location pos);
        void Delete(string id);
    }
}