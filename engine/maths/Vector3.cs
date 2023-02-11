using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alice.engine.maths
{

    public class Vector3
    {

        public float X, Y, Z;

        public Vector3(Vector2 vector2) 
        {
            X = vector2.X;
            Y = vector2.Y;
            Z = 0;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal Microsoft.Xna.Framework.Vector3 ToXna()
        {
            return new Microsoft.Xna.Framework.Vector3(X, Y, Z);
        }
    }

}
