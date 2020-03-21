using Web_Api.Entities;
using System;

namespace Web_Api.Interfaces
{
    public interface IDatabase
    {
        bool Contains(Guid id);
        void Create(Guid id, Position pos);
        void Update(Guid id, Position pos);
        void Delete(Guid id);
    }
}