using System;

namespace Web_Api.Dependencies
{
    public static class MathUtil
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}