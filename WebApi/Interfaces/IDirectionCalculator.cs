using Web_Api.Controllers;
using Web_Api.Entities;

namespace Web_Api.Interfaces
{
    public interface IDirectionCalculator
    {
        Directions CalculateDirection(Location me, Location other);
    }
}