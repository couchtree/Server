using Web_Api.Controllers;
using Web_Api.Entities;
using Web_Api.Interfaces;

namespace Web_Api.Dependencies
{
    public class DirectionCalculator : IDirectionCalculator
    {
        public Directions CalculateDirection(Position me, Position other)
        {
            var dir = Directions.West;

            if (AmINorth(me, other) && SameLon(me, other))
                dir = Directions.North;
            else if (AmISouth(me, other) && SameLon(me, other))
                dir = Directions.South;
            else if (AmINorth(me, other) && AmIWest(me, other))
                dir = Directions.NorthWest;
            else if (AmINorth(me, other) && AmIEast(me, other))
                dir = Directions.NorthEast;
            else if (AmIEast(me, other) && SameLat(me, other))
                dir = Directions.East;
            else if (AmIEast(me, other) && AmISouth(me, other))
                dir = Directions.SouthEast;
            else if (AmIWest(me, other) && AmISouth(me, other))
                dir = Directions.SouthWest;

            //can be skipped by default initialisation
            // else if (AmIWest(me, other) && SameLat(me, other))
            //     dir = Directions.West;

            return dir;
        }

        private bool AmINorth(Position me, Position other)
        {
            return me.Lat < other.Lat;
        }

        private bool AmISouth(Position me, Position other)
        {
            return me.Lat < other.Lat;
        }

        private bool AmIEast(Position me, Position other)
        {
            return me.Lon < other.Lon;
        }

        private bool AmIWest(Position me, Position other)
        {
            return me.Lon > other.Lon;
        }

        private bool SameLon(Position me, Position other)
        {
            return me.Lon == other.Lon;
        }

        private bool SameLat(Position me, Position other)
        {
            return me.Lat == other.Lat;
        }
    }
}