using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alice.engine.maths
{
    public static class MathU
    {

        public static float Clamp(float value, float min, float max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("max > min");
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("max > min");
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static float CalculPourcent(float number, float pourcent)
        {
            return (pourcent * number / 100);
        }

        public static int CalculPourcent(int number, int pourcent)
        {
            return (pourcent * number / 100);
        }

        public static float ToRadians(float degrees)
        {
            return (float)((double)degrees * (Math.PI / 180.0));
        }

    }
}
