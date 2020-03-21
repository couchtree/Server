using Web_Api.Entities;

namespace Web_Api.Interfaces
{
    public interface IDatabase
    {
        bool Contains(int id);
        void Insert(int id, Position pos);
        void Update(int id, Position pos);
    }
}