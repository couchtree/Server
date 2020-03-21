using System;
using Web_Api.Entities;

namespace Web_Api.Interfaces
{
    public interface IDatabase
    {
        bool Contains(Guid id);
        void Create(Guid id, Location pos);
        void Update(Guid id, Location pos);
        void Delete(Guid id);
    }
}